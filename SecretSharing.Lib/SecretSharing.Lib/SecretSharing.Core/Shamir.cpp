#pragma once
#include "stdafx.h"
#include "Shamir.h"
#include "ShamirShare.h"
#include <vector>
#include <msclr\lock.h>
using namespace System::Collections::Generic;
using namespace System::Text;
using namespace SecretSharingCore::Common;
using namespace SecretSharingCore::Algorithms;
using namespace NTL;
using namespace System::Threading;
		Shamir::Shamir(){

		}




		List<IShareCollection^>^ Shamir::DivideSecret(int K, int N, long Secret){
			int size = sizeof(long);
			array<Byte>^ arrayOfByte = BitConverter::GetBytes(Secret);
			/*if (BitConverter::IsLittleEndian)
				Array::Reverse(arrayOfByte);*/
			Nullable<double> a = NULL;
			return DivideSecret(K, N, arrayOfByte ,(Byte)size,a);
		}

		double Shamir::GetPrimeGenerationTime(){
			return this->primeGenTime;
		}
		
		List<IShare^>^ Shamir::DivideSecret(int K, int N, array<Byte>^ Secret, int StartIndex, int ChunkSize, Nullable<double>% TimeElapsedForPrimeGeneration)
		{
			pin_ptr<unsigned char> unmanagedSecretArray = &Secret[StartIndex];
			ZZ chunkSecret = ZZFromBytes(unmanagedSecretArray, ChunkSize);

			//cout << "chunkSecret:"<<chunkSecret<<'\n';

			DateTime currentTime = DateTime::Now;
			

			//Monitor::Enter(m_lock);
			//generate prime p
			ZZ p = RandomPrime_ZZ((ChunkSize*8)+1);

			//cout << "prime:" << p << '\n';
			//cout << "prime number is:" << p << '\n';
			while (p < chunkSecret){
				RandomPrime(p, (ChunkSize * 8) + 1);
			//	cout << "prime:" << p << '\n';
			}
			//Monitor::Exit(m_lock);

			TimeSpan tp = DateTime::Now - currentTime;
			primeGenTime = tp.TotalMilliseconds;
			if (TimeElapsedForPrimeGeneration.HasValue) TimeElapsedForPrimeGeneration = tp.TotalMilliseconds;

			ZZ_p::init(p);
			Vec<ZZ> coefficients = vec_ZZ();
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
			//cout <<"g(x):"<< f;

			
			List<IShare^>^ shares = gcnew List<IShare ^>();
			for (int i = 1; i <= N; i++)
			{
				ZZ* primePtr = new ZZ(p);
				ZZ_p *yz = new ZZ_p(eval(f, ZZ_p(i)));

				//cout<<'\n' <<"yz:"<<*yz<<'\n';
				ShamirShare^ sh = gcnew ShamirShare(i, yz, primePtr);
				shares->Add(sh);
			}

			return shares;
		}

		List<IShareCollection^>^ Shamir::DivideSecret(int K, int N, array<Byte>^ Secret, int ChunkSize, Nullable<Double>% TimeElapsedForPrimeGeneration)
		{
			if (Secret->Length % ChunkSize != 0)
			{
				throw gcnew System::Exception("Secret array must be dividable to Chunk size");
			}
			List<IShareCollection^>^ shares = gcnew List<IShareCollection^>();
			for (int i = 0; i*ChunkSize <Secret->Length; i++)
			{
				List<IShare^>^ currentLetterShares = DivideSecret(K, N, Secret, i*ChunkSize, ChunkSize, TimeElapsedForPrimeGeneration);
				ShareCollection::ScatterShareIntoCollection(currentLetterShares, shares, i);
			}
			return shares;
		}

		long Shamir::ReconstructSecret(List<IShareCollection^>^ Shares){
			int size = sizeof(long);
			array<Byte>^ resultArray = gcnew array<Byte>(size);
			array<Byte>^ secret = ReconstructSecret(Shares, (Byte)size);
			for (int i = 0; i < size;i++)
			{
				if (i < secret->Length)
					resultArray[i] = secret[i];
				else
					resultArray[i] = 0;
			}
			return BitConverter::ToInt32(resultArray, 0);
		}
		
		List<IShareCollection^>^ Shamir::DivideStringSecret(int K, int N, String^ Secret,int ChunkSize){
			List<IShareCollection^>^ shares = gcnew List<IShareCollection^>();
			array<Byte>^ bytes = Encoding::UTF8->GetBytes(Secret->ToCharArray());
			Nullable<double> a = NULL;
			return DivideSecret(K, N, bytes, ChunkSize,a);
		}
		String^ Shamir::ReconstructStringSecret(List<IShareCollection^>^ shareCollections,int ChunkSize){
			array<Byte>^ secret = ReconstructSecret(shareCollections,ChunkSize);
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

				//cout << '\n' << "interpolat val x,y :" << ZZ_p(currentShare->GetX()) << ',' << *currentShare->GetZZ() << '\n';
			}

			ZZ_pX interpolatedf = interpolate(x, y);
			//cout << "interpol g(x):" << interpolatedf;


			ZZ_p secretz = eval(interpolatedf, ZZ_p(0));

			//cout << "reconChunkSecret:" << secretz << '\n';

			return secretz;
		}
		
		array<Byte>^ Shamir::ReconstructSecret(List<IShare^>^ Shares, int ChunkSize){

			ZZ_p secretz =InterpolateSecret(Shares);
			ZZ secret;
			unsigned char* unmanagedSecretArray = (unsigned char*)malloc(sizeof(unsigned char) * ChunkSize);
			conv(secret, secretz);
			BytesFromZZ(unmanagedSecretArray, secret, (long)ChunkSize);
			array<Byte>^ _Data = gcnew array<Byte>(ChunkSize);
			System::Runtime::InteropServices::Marshal::Copy(IntPtr((void *)unmanagedSecretArray), _Data, 0, ChunkSize);
			
			delete unmanagedSecretArray;
			return _Data;
		}
		array<Byte>^  Shamir::ReconstructSecret(List<IShareCollection^>^ shareCollections,int ChunkSize){
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
