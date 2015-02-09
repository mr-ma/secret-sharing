#pragma once
#include "stdafx.h"
#include "IShareCollection.h"
#include "QualifiedSubset.h"
#include "Trustee.h"
using namespace System;
using namespace System::Collections::Generic;
using namespace std;
using namespace SecretSharingCore::Common;

namespace SecretSharingCore
{
	namespace Algorithms
	{
		namespace GeneralizedAccessStructure{
			public ref class ThresholdSubset{
			public:
				int N;
				int K;
				ThresholdSubset::ThresholdSubset(int N, int K, IEnumerable<Trustee^>^ fixedParties, IEnumerable<Trustee^>^ thresholdParties);
				IEnumerable<Trustee^>^ thresholdParties;
				IEnumerable<Trustee^>^ fixedParties;
				virtual bool Equals(Object^ o) override;
				virtual int GetHashCode() override;
				virtual String^ ToString() override;
				IEnumerable<QualifiedSubset^>^ ThresholdSubset::GetQualifiedSubsets();
				List<QualifiedSubset^>^ ThresholdSubset::permutation(int k, IEnumerable<Trustee^>^ persons);
				IEnumerable<Trustee^>^ ThresholdSubset::swap(IEnumerable<Trustee^>^ s, int i, int j);
			};
		}
	}
}