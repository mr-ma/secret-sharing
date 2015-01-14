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
			unsigned long primeGenTime;
			static Object^ m_lock = gcnew Object();
			unsigned long prime;
			long primeLength = 17;
			const long coefficientLength = 5913;
			List<IShare^>^ Shamir::DivideSecret(int K, int N, array<Byte>^ Secret, int StartIndex, Byte ChunkSize);
			array<Byte>^ Shamir::ReconstructSecret(List<IShare^>^ Shares, Byte ChunkSize);
			ZZ_p Shamir::InterpolateSecret(List<IShare^>^ Shares);
		public:
			Shamir::Shamir();
			List<IShareCollection^>^ Shamir::DivideSecret(int K, int N, long Secret);
			long Shamir::ReconstructSecret(List<IShareCollection^>^ Shares);
			List<IShareCollection^>^ Shamir::DivideStringSecret(int K, int N, String^ Secret,Byte ChunkSize);
			String^ Shamir::ReconstructStringSecret(List<IShareCollection^>^ shareCollections,Byte ChunkSize);
			List<IShareCollection^>^ Shamir::DivideSecret(int K, int N, array<Byte>^ Secret, Byte ChunkSize);
			array<Byte>^  Shamir::ReconstructSecret(List<IShareCollection^>^ shareCollections, Byte ChunkSize);
			unsigned long Shamir::GetPrimeGenerationTime();
		};
	}
}