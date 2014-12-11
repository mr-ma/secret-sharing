#include "ISecretShare.h"
#include <NTL/ZZ_pXFactoring.h>
#include "IShareCollection.h"
#pragma once

using namespace System::Collections::Generic;
using namespace NTL;
using namespace std;

		public ref class Shamir :ISecretShare{
		private:
			const long primeLength = 18;
			const long coefficientLength = 59123;
		public:
			Shamir::Shamir();
			virtual List<IShare^>^ Shamir::DivideSecret(int K, int N, int Secret);
			virtual int Shamir::ReconstructSecret(List<IShare^>^ Shares);

			List<IShareCollection^>^ Shamir::DivideSecret(int K, int N, String^ Secret);
			String^ Shamir::ReconstructSecret(List<IShareCollection^>^ shareCollections);
		};
