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
		ZZ p = RandomPrime_ZZ(secret->Length*8 +1);
		cout << "prime:" << p<<'\n';
		ZZ_p::init(p);

		//generate l-1 random numbers
		//Vec<ZZ> randoms = vec_ZZ();
		ZZ sum = ZZ(0);
		for (int i = 0; i < l-1; i++)
		{
			//make sure random coeffcients are smaller than p
			ZZ r;
			RandomBits(r, secret->Length*8);
			//cout << "random coeff:" << r << '\n';
			while (r >= p){
				RandomBits(r, secret->Length*8);
				//cout << "random coeff:"<< r<<'\n';
			}
			cout <<"r: "<< r<<'\n';

			sum += r;
		//	randoms.put(i, r);

			ZZ* primePtr = new ZZ(p);
			ZZ_p *yz = new ZZ_p(to_ZZ_p(r));
			IShare^ share = gcnew ShamirShare(0, yz, primePtr);
			
			shares->Add(share);
		}

		//randoms.put(l, -sum);
		ZZ r;
		cout << "sum: " << sum << '\n';
		if (sum <= SecretZZ){
			 r = SecretZZ-sum;
			
			//randoms.put(l, -sum + SecretZZ);
		}
		else{
			r =-( sum - SecretZZ);
			//randoms.put(l, sum + SecretZZ);
		}
		cout << "last r: " << r << '\n';
		sum += r;
		cout << "last sum: " << sum << '\n';
		cout << "secret" << SecretZZ<<'\n';
		if (sum != SecretZZ) throw gcnew System::Exception("benaloh failed");

		ZZ* primePtr = new ZZ(p);
		ZZ_p *yz = new ZZ_p(to_ZZ_p(r));
		IShare^ share = gcnew ShamirShare(0, yz, primePtr);
		shares->Add(share);
		cout << "share y" << *((ShamirShare^)share)->GetZZ() << '\n';
		//distribute shares
	}
	return shares;
}

array<Byte>^ BenalohLeichter::ReconstructSecret(List<IShare^>^ shares){
	ZZ_p sum = ZZ_p(0);
	for each (IShare^ sh in shares)
	{
		ShamirShare^ share = (ShamirShare^)sh;
		ZZ_p::init(*share->GetPrime());
		sum += *share->GetZZ() ;
		cout << "last sum: " << sum << '\n';
	}

	ZZ secret;
	conv(secret, sum);
	ShamirShare^ s = gcnew ShamirShare(0, 0);
	return 	s->GetArrayOfZZ(secret);
}