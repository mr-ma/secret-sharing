#pragma once
#include "stdafx.h"
#include "IShareCollection.h"
#include "QualifiedSubset.h"
#include "Trustee.h"
using namespace std;
using namespace System;
using namespace System::Collections::Generic;
using namespace SecretSharingCore::Common;

namespace SecretSharingCore
{
	namespace Algorithms
	{
		namespace GeneralizedAccessStructure{

			public ref class AccessStructure{
				private:
					int AccessStructure::getMax(QualifiedSubset^ qs);
			public:
				AccessStructure::AccessStructure();
				AccessStructure::AccessStructure(String^ minimalAccessPath);
				List<QualifiedSubset^>^  Accesses;
				List<Trustee^>^ AccessStructure::GetAllParties();
				int AccessStructure::GetLongestLength();
				
			};
		}
	}
}