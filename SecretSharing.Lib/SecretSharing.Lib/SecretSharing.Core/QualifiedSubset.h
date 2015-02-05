#pragma once
#include "stdafx.h"
#include "IShareCollection.h"
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
			public ref class QualifiedSubset{
			public:
				QualifiedSubset :: QualifiedSubset();
				List<Trustee^>^ Parties;
				virtual bool Equals(Object^ o) override;
				virtual int GetHashCode() override;
				void AssignSecretPercentage();
			};
		}
	}
}