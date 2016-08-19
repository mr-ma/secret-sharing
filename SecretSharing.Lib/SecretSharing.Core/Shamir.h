#pragma once
#include <NTL/ZZ_pXFactoring.h>
#include "IShareCollection.h"
#include <msclr\lock.h>
using namespace System;
using namespace System::Collections::Generic;
using namespace NTL;
using namespace std;
using namespace SecretSharingCore::Common;

namespace SecretSharingCore
{
	namespace Algorithms{
		public ref class Shamir :ISecretShare{
		private:
			double primeGenTime;
			static Object^ m_lock = gcnew Object();
			unsigned long prime;
			List<IShare^>^ Shamir::DivideSecret(int K, int N, array<Byte>^ Secret, int StartIndex, int ChunkSize
#ifdef calcPrimeTime
				,TimeSpan% TimeElapsedForPrimeGeneration
#endif
				);
			array<Byte>^ Shamir::ReconstructSecret(List<IShare^>^ Shares, int ChunkSize);
			ZZ_p Shamir::InterpolateSecret(List<IShare^>^ Shares);
		public:
			Shamir::Shamir();
			List<IShareCollection^>^ Shamir::DivideSecret(int K, int N, long Secret);
			//long Shamir::ReconstructSecret(List<IShareCollection^>^ Shares);
			List<IShareCollection^>^ Shamir::DivideStringSecret(int K, int N, String^ Secret,int ChunkSize);
			String^ Shamir::ReconstructStringSecret(List<IShareCollection^>^ shareCollections,int ChunkSize);
			
			List<IShareCollection^>^ Shamir::DivideSecret(int K, int N, array<Byte>^ Secret, int ChunkSize
#ifdef calcPrimeTime
				, 
				double% TimeElapsedForPrimeGeneration
#endif
					);
			
			List<IShareCollection^>^ Shamir::DivideSecret(int K, int N, array<Byte>^ Secret);
			array<Byte>^  Shamir::ReconstructSecret(List<IShareCollection^>^ shareCollections);

			array<Byte>^  Shamir::ReconstructSecret(List<IShareCollection^>^ shareCollections, int ChunkSize);
		};
	}
}