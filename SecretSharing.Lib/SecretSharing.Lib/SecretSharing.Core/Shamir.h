#pragma once
#include <NTL/ZZ_pXFactoring.h>
#include "IShareCollection.h"

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
			unsigned long prime;
			long primeLength = 17;
			const long coefficientLength = 5913;
			List<IShare^>^ Shamir::DivideSecret(int K, int N, array<Byte>^ Secret, int StartIndex, Byte ChunkSize);
			array<Byte>^ Shamir::ReconstructSecret(List<IShare^>^ Shares, Byte ChunkSize);
			ZZ_p Shamir::InterpolateSecret(List<IShare^>^ Shares);
		public:
			Shamir::Shamir();
			virtual List<IShare^>^ Shamir::DivideSecret(int K, int N, long Secret);
			virtual long Shamir::ReconstructSecret(List<IShare^>^ Shares);
			virtual long Shamir::GetPrime();
			List<IShareCollection^>^ Shamir::DivideSecret(int K, int N, String^ Secret);
			String^ Shamir::ReconstructSecret(List<IShareCollection^>^ shareCollections);
			List<IShareCollection^>^ Shamir::DivideSecret(int K, int N, array<Byte>^ Secret, Byte ChunkSize);
			array<Byte>^  Shamir::ReconstructSecret(List<IShareCollection^>^ shareCollections, Byte ChunkSize);
		};
	}
}