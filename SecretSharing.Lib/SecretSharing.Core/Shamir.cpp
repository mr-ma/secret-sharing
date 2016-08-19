#pragma once
#include "stdafx.h"
#include "Shamir.h"
#include "ShamirShare.h"
#include "NTLHelper.h"
#include <vector>
#include <msclr\lock.h>
#include <time.h>
#include "PrimeGenerator.h"
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
#ifdef calcPrimeTime
		    double a = 0;
			return DivideSecret(K, N, arrayOfByte ,(Byte)size,a);
#else 
			return DivideSecret(K, N, arrayOfByte);
#endif
		}

		
		List<IShare^>^ Shamir::DivideSecret(int K, int N, array<Byte>^ Secret, int StartIndex, int ChunkSize
#ifdef calcPrimeTime
			, TimeSpan% TimeElapsedForPrimeGeneration
#endif
			)
		{
			pin_ptr<unsigned char> unmanagedSecretArray = &Secret[StartIndex];
			ZZ chunkSecret = ZZFromBytes(unmanagedSecretArray, ChunkSize);

#ifdef calcPrimeTime
			DateTime currentTime = DateTime::Now;
#endif

			//set the random seed
			SetSeed(conv<ZZ>((long)time(0)));
			//generate prime p
			PrimeGenerator^ primeGenerator = gcnew PrimeGenerator();
			ZZ p = primeGenerator->LoadOrGenerateZZRandomPrime(ChunkSize); //  RandomPrime_ZZ((ChunkSize*8)+1);
<<<<<<< HEAD:SecretSharing.Lib/SecretSharing.Core/Shamir.cpp
			while (p < chunkSecret) {
				//throw gcnew Exception("Prime field is smaller than the secret!");
				 p = primeGenerator->LoadOrGenerateZZRandomPrime(ChunkSize+1);
			}
=======
			if (p < chunkSecret) throw gcnew Exception("Prime field is smaller than the secret!");
>>>>>>> f2b0505944f079bd6723813d2eef047a39fbf227:SecretSharing.Lib/SecretSharing.Lib/SecretSharing.Core/Shamir.cpp
#ifdef calcPrimeTime
			TimeElapsedForPrimeGeneration = DateTime::Now - currentTime;
#endif
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
List<IShareCollection^>^ Shamir::DivideSecret(int K, int N, array<Byte>^ Secret, int ChunkSize
#ifdef calcPrimeTime
			, Double% TimeElapsedForPrimeGeneration
#endif	
			){
			if (Secret->Length % ChunkSize != 0)
			{
				throw gcnew System::Exception("Secret array must be dividable to Chunk size");
			}
			List<IShareCollection^>^ shares = gcnew List<IShareCollection^>();
			TimeSpan PrimeTimeSpan;
			for (int i = 0; i*ChunkSize <Secret->Length; i++)
			{
#ifdef calcPrimeTime
				List<IShare^>^ currentLetterShares = DivideSecret(K, N, Secret, i*ChunkSize, ChunkSize, PrimeTimeSpan);
				TimeElapsedForPrimeGeneration += PrimeTimeSpan.Ticks;
#else

				List<IShare^>^ currentLetterShares = DivideSecret(K, N, Secret, i*ChunkSize, ChunkSize);
#endif
				ShareCollection::ScatterShareIntoCollection(currentLetterShares, shares, i);
			}
			return shares;
		}


		List<IShareCollection^>^ Shamir::DivideSecret(int K, int N, array<Byte>^ Secret){

			Double TimeElapsedForPrimeGeneration = 0;
			return DivideSecret(K, N, Secret, Secret->Length
#ifdef calcPrimeTime	
				,TimeElapsedForPrimeGeneration
#endif			
			);
		}
		array<Byte>^  Shamir::ReconstructSecret(List<IShareCollection^>^ shareCollections){
			List<IShare^>^ currentLetterList = ShareCollection::GatherShareFromCollection(shareCollections, 0);
			ZZ_p secretz = InterpolateSecret(currentLetterList);

			return  NTLHelper::ZZpToByte(secretz);
		}


		List<IShareCollection^>^ Shamir::DivideStringSecret(int K, int N, String^ Secret,int ChunkSize){
			List<IShareCollection^>^ shares = gcnew List<IShareCollection^>();
			array<Byte>^ bytes = Encoding::UTF8->GetBytes(Secret->ToCharArray());
#ifdef calcPrimeTime
			double a = 0;
			return DivideSecret(K, N, bytes, ChunkSize,a);
#else
			return DivideSecret(K, N, bytes, ChunkSize);
#endif
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
