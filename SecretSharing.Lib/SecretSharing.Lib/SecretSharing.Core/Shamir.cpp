#pragma once
#include "stdafx.h"
#include "Shamir.h"
#include "ShamirShare.h"
using namespace System::Collections::Generic;
using namespace System::Text;
using namespace SecretSharingCore::Common;
using namespace SecretSharingCore::Algorithms;
using namespace NTL;
		Shamir::Shamir(){

		}
		List<IShare^>^ Shamir::DivideSecret(int K, int N, long Secret){

			//generate prime p
			ZZ p = GenGermainPrime_ZZ(primeLength);
			//cout << "prime number is:" << p << '\n';
			while (p < Secret){
				primeLength++;
				p = GenGermainPrime_ZZ(primeLength);
			}
			
			
			unsigned long a;
			conv(a, p);
			prime = a;

			ZZ_p::init(p);
			Vec<ZZ> coefficients = vec_ZZ();
			ZZ maxRandom = ZZ(coefficientLength);
			ZZ_pX f;
			f.SetLength(K);
			SetCoeff(f, 0, ZZ_p(Secret));
			for (int i = 1; i < K; i++)
			{
				ZZ r = RandomBnd(maxRandom);
				//cout << "random coeff number is:" << r << '\n';
				SetCoeff(f, i, to_ZZ_p(r));
			}

			//cout << "F is:" << f << '\n';
			List<IShare^>^ shares = gcnew List<IShare ^>();
			for (int i = 1; i <= N; i++)
			{
				ZZ_p *yz = new ZZ_p(eval(f, ZZ_p(i)));
				//ZZ_p yz = eval(f, ZZ_p(i));
		/*		unsigned long y;
				conv(y, yz);
				if (y < 0 ) throw gcnew Exception(String::Format("Overflow in evaluating polynomial f with x={0}",i));*/
				ShamirShare^ sh = gcnew ShamirShare(i, yz);
				shares->Add(sh);
			}

			return shares;
		}
		long Shamir::ReconstructSecret(List<IShare^>^ Shares){
			Vec<ZZ_p> y = vec_ZZ_p();
			Vec<ZZ_p> x = vec_ZZ_p();

			int count = Shares->Count;
			for (int i = 0; i < count; i++)
			{
				ShamirShare^ currentShare = (ShamirShare^) Shares[i];
				x.append(ZZ_p(currentShare->GetX()));
			/*	ZZ_p yz;
				conv(yz, Shares[i]->GetY());*/
				y.append(*currentShare->GetZZ() );
			}
			ZZ_pX interpolatedf = interpolate(x, y);
			cout << "interpolated f:" << interpolatedf << '\n';

			ZZ_p secretz = eval(interpolatedf, ZZ_p(0));
			unsigned long secret;
			conv(secret, secretz);
			return secret;
		}
		
		List<IShareCollection^>^ Shamir::DivideSecret(int K, int N, String^ Secret){
			List<IShareCollection^>^ shares = gcnew List<IShareCollection^>();

			array<Byte>^ bytes = Encoding::UTF8->GetBytes(Secret->ToCharArray());
			for (int i = 0; i<bytes->Length; i++)
			{
				List<IShare^>^ currentLetterShares = DivideSecret(K,N, (int)bytes[i]);
				ShareCollection::ScatterShareIntoCollection(currentLetterShares, shares, i);
			}

			return shares;
		}
		String^ Shamir::ReconstructSecret(List<IShareCollection^>^ shareCollections){
			int count = shareCollections[0]->GetCount();
			array<Byte>^ secret = gcnew array<Byte>(count);
			for (int i = 0; i < count; i++)
			{
				List<IShare^>^ currentLetterList = ShareCollection::GatherShareFromCollection(shareCollections, i);
				int currentSecretLetter = ReconstructSecret(currentLetterList);
				secret[i] = currentSecretLetter;
			}
			return Encoding::UTF8->GetString(secret);

		}

		long Shamir::GetPrime(){
			return this->prime;

		}
