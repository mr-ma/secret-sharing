#pragma once
#include "stdafx.h"
#include "NTL\ZZ.h"
#include <vector>
#include <tuple>
#include "NativePtr.h"
#include "SchoenmakersShare.h"
using namespace System::Collections::Generic;
using namespace System;
using namespace SecretSharing::OptimalThreshold::Models;
using namespace NTL;
using namespace std;
namespace SecretSharingCore
{
	namespace Algorithms{
		namespace PVSS{
			public ref class Schoenmakers
			{
			private :
				int fieldSizeInByte;
				CAutoNativePtr<ZZ> q; //the prime field
				CAutoNativePtr<ZZ_p> g; //primitive generator of q
				CAutoNativePtr<ZZ_p> G; //another primitive generator of q
				CAutoNativePtr<vector<ZZ_p>> y; //public keys
			public:
				Schoenmakers::Schoenmakers();
				void Schoenmakers::SetPublicKeys(vector<ZZ_p> y);
				void Schoenmakers::SetPublicKeys(List<array<Byte>^>^ y);

				void Schoenmakers::SelectPrimeAndGenerators(int fieldSizeInByte);
				void Schoenmakers::Distribute(int t, int n, vector<ZZ_p>% EncryptedShares, vector<ZZ_p>% Commitments, vector<ZZ_p>% r, vector<ZZ_p>% c, ZZ_p% secret);
				List<SchoenmakersShare^>^ Schoenmakers::Distribute(int t, int n, List<array<Byte>^>^% Commitments, array<Byte>^% secret);
				List<SchoenmakersShare^>^ Schoenmakers::Distribute(int t, int n, array<Byte>^ sigma, List<array<Byte>^>^% Commitments, array<Byte>^% secret, array<Byte>^% U);

				bool Schoenmakers::VerifyDistributedShare(int i, ZZ_p r, ZZ_p c, ZZ_p Y, vector<ZZ_p> Commitments, ZZ_p y);
				bool Schoenmakers::VerifyDistributedShare(int i, SchoenmakersShare^ share, List<array<Byte>^>^ Commitments, array<Byte>^ y);

				// PoolShare returns decrypted share ( G ^ p(i) ) and proofs of having xi
				ZZ_p Schoenmakers::PoolShare(ZZ_p x, ZZ_p y, ZZ_p Y, ZZ_p% r, ZZ_p% c); //r and c and proofs that party generate to prove he has the private key
				void Schoenmakers::PoolShare(Tuple< array<Byte>^ , array<Byte>^ >^ keypair, SchoenmakersShare^% share);

				bool Schoenmakers::VerifyPooledShares(int t, vector<ZZ_p> S, vector<ZZ_p> Y, vector<ZZ_p> r, vector<ZZ_p> c);
				bool Schoenmakers::VerifyPooledShares(int t, List<SchoenmakersShare^>^ shares);

				ZZ_p Schoenmakers::Reconstruct(int t, vector<ZZ_p> Shares);
				array<Byte>^ Schoenmakers::Reconstruct(int t, List<SchoenmakersShare^>^ Shares);
				array<Byte>^ Schoenmakers::Reconstruct(int t, List<SchoenmakersShare^>^ Shares, array<Byte>^ U);

				ZZ Schoenmakers::Getq();
				ZZ_p Schoenmakers::Getg();
				ZZ_p Schoenmakers::GetG();
				array<Byte>^ Schoenmakers::GetqB();
				array<Byte>^ Schoenmakers::GetgB();
				array<Byte>^ Schoenmakers::GetGB();
			};
		}
	}
}

