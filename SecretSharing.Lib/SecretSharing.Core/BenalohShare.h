#pragma once
#include "ShamirShare.h"
using namespace SecretSharing::OptimalThreshold::Models;

namespace SecretSharingCore
{
	namespace Algorithms{
		namespace GeneralizedAccessStructure{
			ref class BenalohShare :
				public ShamirShare
			{
			private:
				ISubset^ subset;
			public:
				BenalohShare(ISubset^ subset, int partyId,int x, array<Byte>^ y, array<Byte>^ p);
				BenalohShare(ISubset^ subset, int partyId,int x, ZZ_p* y, ZZ* prime);
			};
		}
	}
}
