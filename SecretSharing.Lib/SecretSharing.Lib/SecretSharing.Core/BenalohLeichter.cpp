#pragma once
#include "stdafx.h"
#include "ShamirShare.h"
#include "BenalohLeichter.h"
#include "QualifiedSubset.h"
#include "AccessStructure.h"
#include "NTL\ZZ.h"
#include "NTL\ZZ_p.h"
#include "NTL\vec_ZZ.h"
#include <vector>
using namespace System;
using namespace System::Collections::Generic;
using namespace std;
using namespace SecretSharingCore::Common;
using namespace SecretSharingCore::Algorithms::GeneralizedAccessStructure;
using namespace NTL;

BenalohLeichter::BenalohLeichter(){
}

List<IShare^>^ BenalohLeichter::DivideSecret(array<Byte>^  secret, AccessStructure^ accessStructure){
	
	pin_ptr<unsigned char> unmanagedSecretArray = &secret[0];
	ZZ SecretZZ = ZZFromBytes(unmanagedSecretArray, secret->Length);

	List<IShare^>^ shares = gcnew List<IShare^>();


	// divide the secret among parties involved foreach access structure
	for each (QualifiedSubset^ var in accessStructure->Accesses)
	{
		///Number of parties
		int l = var->Parties->Count;
		ZZ p = RandomPrime_ZZ(secret->Length +1);
		ZZ_p::init(p);

		//generate l-1 random numbers
		Vec<ZZ> randoms = vec_ZZ();
		ZZ sum = ZZ(0);
		for (int i = 0; i < l-1; i++)
		{
			//make sure random coeffcients are smaller than p
			ZZ r;
			RandomBits(r, secret->Length);
			//cout << "random coeff:" << r << '\n';
			while (r >= p){
				RandomBits(r, secret->Length);
				//cout << "random coeff:"<< r<<'\n';
			}
			sum += r;
			randoms.put(i, r);
			ZZ* primePtr = new ZZ(p);
			ZZ_p *yz = new ZZ_p(to_ZZ_p(r));
			IShare^ share = gcnew ShamirShare(0, yz, primePtr);
			
			shares->Add(share);
		}

		randoms.put(l, -sum);

		if (sum <= 0){
			randoms.put(l, -sum + SecretZZ);
		}
		else{
			randoms.put(l, sum + SecretZZ);
		}
		
		//distribute shares
	}
	return shares;
}

array<Byte>^ BenalohLeichter::ReconstructSecret(List<IShare^>^ shares){
	return nullptr;

}