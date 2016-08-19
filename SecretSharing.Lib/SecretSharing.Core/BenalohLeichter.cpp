#pragma once
#include "stdafx.h"
#include "BenalohShare.h"
#include "BenalohLeichter.h"
#include "PrimeGenerator.h"
#include "Shamir.h"
#include "NTL\ZZ.h"
#include "NTL\ZZ_p.h"
#include "NTL\vec_ZZ.h"
#include "NTLHelper.h"
#include <vector>
using namespace SecretSharing::OptimalThreshold::Models;
using namespace System;
using namespace System::Collections::Generic;
using namespace std;
using namespace SecretSharingCore::Common;
using namespace SecretSharingCore::Algorithms::GeneralizedAccessStructure;
using namespace NTL;


BenalohLeichter::BenalohLeichter(){
}

//TODO implement a sharecollection for Benaloh as a each party can have multiple shares
List<IShareCollection^>^ BenalohLeichter::DivideSecret(array<Byte>^  secret, AccessStructure^ accessStructure){
	
	ZZ SecretZZ = NTLHelper::ByteToZZ(secret);

	

	List<IShareCollection^>^ allshares = gcnew List<IShareCollection^>();
	// divide the secret among parties involved foreach access structure
	for each (ISubset^ var in accessStructure->Accesses)
	{
		List<IShare^>^ currentShares = gcnew List<IShare^>();

		///Number of parties
		int l = var->getShareBranchesCount();

		PrimeGenerator^ pg = gcnew PrimeGenerator();
		ZZ p = pg->LoadOrGenerateZZRandomPrime(secret->Length); //RandomPrime_ZZ(secret->Length*8 +1);
		if (SecretZZ > p){
			throw  gcnew Exception("Prime field is smaller than the secret!");
		}
		//cout << "prime:" << p<<'\n';
		ZZ_p::init(p);

		//generate l-1 random numbers
		ZZ sum = ZZ(0);
		for (int i = 0; i < l-1; i++)
		{
			//make sure random coeffcients are smaller than p
			ZZ r;
			RandomBits(r, secret->Length*8);
			while (r >= p){
				RandomBits(r, secret->Length*8);
			}
			//cout <<"r: "<< r<<'\n';

			sum += r;

			ZZ* primePtr = new ZZ(p);
			ZZ_p *yz = new ZZ_p(to_ZZ_p(r));
			IShare^ share = gcnew BenalohShare(var,var->getPartyId(i),0, yz, primePtr);
			
			currentShares->Add(share);
		}
		
		


		ZZ r;
		//cout << "sum: " << sum << '\n';
		if (sum <= SecretZZ){
			 r = SecretZZ-sum;
		}
		else{
			r =-( sum - SecretZZ);
		}
		//cout << "last r: " << r << '\n';
		sum += r;
		//cout << "last sum: " << sum << '\n';
		//cout << "secret" << SecretZZ<<'\n';
		if (sum != SecretZZ)
		{
			throw gcnew System::Exception("Failed to divide secret");
		}

		

		ZZ* primePtr = new ZZ(p);
		ZZ_p r_zz_p = to_ZZ_p(r);
		ZZ_p *yz = new ZZ_p(r_zz_p);
		//cout << "yz: "<< *yz<<'\n';



		if (var->GetType() == ThresholdSubset::typeid){
			array<Byte>^ thresholdsecret = NTLHelper::ZZToByte(r);
			//cout << "r_zz " << r_zz_p<<'\n';
			//cout << "here you go " << NTLHelper::ByteToZZ(thresholdsecret) << '\n';
			currentShares->AddRange( DivideThresholdSecret(thresholdsecret, dynamic_cast<ThresholdSubset^>(var)));
			
			IShare^ share = gcnew BenalohShare(var, -1, 0, new ZZ_p(0), primePtr);
			currentShares->Add(share);
		}
		else{
			IShare^ share = gcnew BenalohShare(var, var->getPartyId(l-1),0, yz, primePtr);
			currentShares->Add(share);
		}
		

		//distribute shares
		ShareCollection^ col = gcnew ShareCollection();
		col->shares->AddRange(currentShares);
		allshares->Add(col);
	}
	return allshares;
}

array<Byte>^ BenalohLeichter::ReconstructSecret(IShareCollection^ shares){
	ZZ_p sum = ZZ_p(0);
	List<IShareCollection^>^ shamirshares = gcnew List<IShareCollection^>();
	for each (IShare^ sh in shares->GetAllShares())
	{
		if (sh->GetType() == BenalohShare::typeid){
			BenalohShare^ share = (BenalohShare^)sh;
			ZZ_p::init(*share->GetPrime());
			sum += *share->GetZZ();
		}
		//get shares of thresholds
		else if (sh->GetType() == ShamirShare::typeid){
			ShareCollection^ col = gcnew ShareCollection ();
			col->shares->Add(sh);
			shamirshares->Add(col);
		}
		//cout << "last sum: " << sum << '\n';
	}

	if (shamirshares->Count > 0){
		array<Byte>^ thresholdsecret = ReconstructThresholdSecret(shamirshares);
		ZZ_p SecretZZp = NTLHelper::ByteToZZ_p(thresholdsecret);
		sum += SecretZZp;

		//cout << "threshold secret: " << SecretZZp << '\n';
		//cout << "last sum: " << sum << '\n';
	}

	return NTLHelper::ZZpToByte(sum);
}


List<IShare^>^ BenalohLeichter::DivideThresholdSecret(array<Byte>^ Secret, ThresholdSubset^ threshold){
	Shamir^ shamir = gcnew Shamir();
	List<IShareCollection^>^ cols =  shamir->DivideSecret(threshold->K, threshold->N, Secret);
	List<IShare^>^ gatheredshares = ShareCollection::GatherShareFromCollection(cols, 0);
	return gatheredshares;
}
array<Byte>^ BenalohLeichter::ReconstructThresholdSecret(List<IShareCollection^>^ shares){
	Shamir^ shamir = gcnew Shamir();
	return shamir->ReconstructSecret(shares);
}