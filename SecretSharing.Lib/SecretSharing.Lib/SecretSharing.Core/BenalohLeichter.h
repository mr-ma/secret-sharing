#pragma once
#include "stdafx.h"
#include "IShareCollection.h"
#include "QualifiedSubset.h"
#include "AccessStructure.h"
using namespace System;
using namespace System::Collections::Generic;
using namespace std;
using namespace SecretSharingCore::Common;
namespace SecretSharingCore
{
	namespace Algorithms{
		namespace GeneralizedAccessStructure{
			public ref class BenalohLeichter : ISecretShare{
			public:
				BenalohLeichter::BenalohLeichter();
				List<IShare^>^ BenalohLeichter::DivideSecret(array<Byte>^ Secret, AccessStructure^ accessStructure);
				array<Byte>^  BenalohLeichter::ReconstructSecret(List<IShare^>^ shares);
			};
		}
	}
}

