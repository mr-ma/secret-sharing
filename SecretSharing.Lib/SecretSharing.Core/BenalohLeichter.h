#pragma once
#include "stdafx.h"
#include "IShareCollection.h"
/*#include "QualifiedSubset.h"
#include "AccessStructure.h"*/
using namespace System;
using namespace System::Collections::Generic;
using namespace std;
using namespace SecretSharingCore::Common;
using namespace SecretSharing::OptimalThreshold::Models;

namespace SecretSharingCore
{
	namespace Algorithms{
		namespace GeneralizedAccessStructure{
			public ref class BenalohLeichter : ISecretShare{
			private :
				List<IShare^>^ BenalohLeichter::DivideThresholdSecret(array<Byte>^ Secret, ThresholdSubset^ threshold);
				array<Byte>^ BenalohLeichter::ReconstructThresholdSecret(List<IShareCollection^>^ shares);
			public:
				BenalohLeichter::BenalohLeichter();
				List<IShareCollection^>^ BenalohLeichter::DivideSecret(array<Byte>^ Secret, AccessStructure^ accessStructure);
				array<Byte>^  BenalohLeichter::ReconstructSecret(IShareCollection^ shares);

			};
		}
	}
}

