#pragma once
#include "stdafx.h"
#include "IShareCollection.h"
#include "QualifiedSubset.h"
#include "AccessStructure.h"
using namespace System;
using namespace System::Collections::Generic;
using namespace std;
using namespace SecretSharingCore::Common;
using namespace SecretSharingCore::Algorithms::BenalohLeichter;
namespace SecretSharingCore
{
	namespace Algorithms{
		public ref class BenalohLeichter :ISecretShare{
		public:
			BenalohLeichter::BenalohLeichter();
			List<IShareCollection^>^ BenalohLeichter::DivideSecret(array<Byte>^ Secret, AccessStructure accessStructure);
			array<Byte>^  BenalohLeichter::ReconstructSecret(List<IShareCollection^>^ shares);
		};
	}
}

