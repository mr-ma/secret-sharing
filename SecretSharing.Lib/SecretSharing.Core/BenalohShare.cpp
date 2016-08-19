#include "stdafx.h"
#include "BenalohShare.h"
using namespace SecretSharingCore::Common;
using namespace SecretSharingCore::Algorithms::GeneralizedAccessStructure;


BenalohShare::BenalohShare(ISubset^ subset, int partyId,int x, array<Byte>^ y, array<Byte>^ p): ShamirShare(x, y, p){
	this->subset = subset;
};
BenalohShare::BenalohShare(ISubset^ subset, int partyId,int x, ZZ_p* y, ZZ* prime) : ShamirShare(x, y, prime){
	this->subset = subset;
};
