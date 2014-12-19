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
				ShamirShare^ sh = gcnew ShamirShare(i, yz, &p);
				shares->Add(sh);
			}

			return shares;
		}

		List<IShare^>^ Shamir::DivideSecret(int K, int N, array<Byte>^ Secret, int StartIndex,Byte ChunkSize)
		{
			pin_ptr<unsigned char> unmanagedSecretArray = &Secret[StartIndex];
			//unsigned char* chunk;
			ZZ chunkSecret = ZZFromBytes(unmanagedSecretArray, ChunkSize);
			//delete unmanagedSecretArray;

			cout << "chunkSecret:"<<chunkSecret<<'\n';

			//generate prime p
			ZZ p;
			RandomPrime(p,ChunkSize*8);

			cout << "prime:" << p << '\n';
			//cout << "prime number is:" << p << '\n';
			while (p < chunkSecret){
				RandomPrime(p, ChunkSize * 8);
	/*			primeLength++;
				p = GenGermainPrime_ZZ(primeLength);*/
				cout << "prime:" << p << '\n';
			}


			ZZ_p::init(p);
			Vec<ZZ> coefficients = vec_ZZ();
			ZZ maxRandom = ZZ(coefficientLength);
			ZZ_pX f;
			f.SetLength(K);
			SetCoeff(f, 0, to_ZZ_p(chunkSecret));
			for (int i = 1; i < K; i++)
			{
				//make sure random coeffcients are smaller than p
				ZZ r;
				RandomBits(r, ChunkSize * 8);
				//cout << "random coeff:" << r << '\n';
				while (r >= p){
					RandomBits(r, ChunkSize * 8);
					//cout << "random coeff:"<< r<<'\n';
				}
				SetCoeff(f, i, to_ZZ_p(r));
			}
			cout <<"g(x):"<< f;

			ZZ* primePtr = new ZZ(p);
			List<IShare^>^ shares = gcnew List<IShare ^>();
			for (int i = 1; i <= N; i++)
			{
				
				ZZ_p *yz = new ZZ_p(eval(f, ZZ_p(i)));

				cout<<'\n' <<"yz:"<<*yz<<'\n';
				//ZZ_p yz = eval(f, ZZ_p(i));
				/*		unsigned long y;
				conv(y, yz);
				if (y < 0 ) throw gcnew Exception(String::Format("Overflow in evaluating polynomial f with x={0}",i));*/
				ShamirShare^ sh = gcnew ShamirShare(i, yz, primePtr);
				shares->Add(sh);
			}

			return shares;
		}

		List<IShareCollection^>^ Shamir::DivideSecret(int K, int N, array<Byte>^ Secret, Byte ChunkSize)
		{
			//pin_ptr<unsigned char> unmanagedSecretArray = &Secret[0];
			//unsigned char* chunk;
			//ZZ chunkSecret = ZZFromBytes(unmanagedSecretArray, ChunkSize);

			List<IShareCollection^>^ shares = gcnew List<IShareCollection^>();
			for (int i = 0; i*ChunkSize <Secret->Length; i++)
			{
				List<IShare^>^ currentLetterShares = DivideSecret(K, N, Secret, i*ChunkSize, ChunkSize);
				ShareCollection::ScatterShareIntoCollection(currentLetterShares, shares, i);
			}
			return shares;
		}

		long Shamir::ReconstructSecret(List<IShare^>^ Shares){
			ZZ_p secretz = InterpolateSecret(Shares);
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


		ZZ_p Shamir::InterpolateSecret(List<IShare^>^ Shares){
			Vec<ZZ_p> y = vec_ZZ_p();
			Vec<ZZ_p> x = vec_ZZ_p();

			int count = Shares->Count;
			for (int i = 0; i < count; i++)
			{
				ShamirShare^ currentShare = (ShamirShare^)Shares[i];
				//init ZZ_p with prime number 
				ZZ_p::init(*currentShare->GetPrime());

				x.append(ZZ_p(currentShare->GetX()));
				y.append(*currentShare->GetZZ());

				cout << '\n' << "interpolat val x,y :" << ZZ_p(currentShare->GetX()) << ',' << *currentShare->GetZZ() << '\n';
			}

			ZZ_pX interpolatedf = interpolate(x, y);
			cout << "interpol g(x):" << interpolatedf;


			ZZ_p secretz = eval(interpolatedf, ZZ_p(0));

			cout << "reconChunkSecret:" << secretz << '\n';

			return secretz;
		}

		array<Byte>^ Shamir::ReconstructSecret(List<IShare^>^ Shares, Byte ChunkSize){

			ZZ_p secretz = InterpolateSecret(Shares);

			unsigned char* unmanagedSecretArray = new unsigned char();
			BytesFromZZ(unmanagedSecretArray, to_ZZ( secretz._ZZ_p__rep), ChunkSize);
			array<Byte>^ _Data = gcnew array<Byte>(ChunkSize);
			System::Runtime::InteropServices::Marshal::Copy(IntPtr((void *)unmanagedSecretArray), _Data, 0, ChunkSize);
			
			delete unmanagedSecretArray;
			return _Data;
		}
		array<Byte>^  Shamir::ReconstructSecret(List<IShareCollection^>^ shareCollections,Byte ChunkSize){
			int count = shareCollections[0]->GetCount();
			array<Byte>^ secret = gcnew array<Byte>(count*ChunkSize);
			for (int i = 0; i < count; i++)
			{
				List<IShare^>^ currentLetterList = ShareCollection::GatherShareFromCollection(shareCollections, i);
				array<Byte>^ currentSecretLetter = ReconstructSecret(currentLetterList, ChunkSize);
				currentSecretLetter->CopyTo(secret, i*ChunkSize);
			}
			return secret;

		}

		long Shamir::GetPrime(){
			return this->prime;

		}
