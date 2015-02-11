#pragma once
#include "stdafx.h"
#include "ThresholdSubset.h"
#include <algorithm>
using namespace System;
using namespace System::Collections::Generic;
using namespace std;
using namespace SecretSharingCore::Common;
using namespace SecretSharingCore::Algorithms::GeneralizedAccessStructure;
using namespace System::Linq;

ThresholdSubset::ThresholdSubset(int N, int K, IEnumerable<Trustee^>^ fixedParties, IEnumerable<Trustee^>^ thresholdParties){
	this->fixedParties = fixedParties;
	this->thresholdParties = thresholdParties;
	this->N = N;
	this->K = K;
}


bool ThresholdSubset::Equals(Object^ obj) { 
	if (obj == nullptr || GetType() != obj->GetType())
		return false;

	ThresholdSubset^ p = dynamic_cast<ThresholdSubset^>(obj);
	if (Enumerable::Count( this->fixedParties ) == Enumerable::Count( p->fixedParties)){

		for each (Trustee^ var in p->fixedParties)
		{
			if (!Enumerable::Contains( this->fixedParties,var))
				return false;
		}
	}
	else return false;
	if (Enumerable::Count(this->thresholdParties) == Enumerable::Count(p->thresholdParties)){

		for each (Trustee^ var in p->thresholdParties)
		{
			if (!Enumerable::Contains(this->thresholdParties, var))
				return false;
		}
	}
	else return false;
	return true;
}
int ThresholdSubset::GetHashCode() { 
	int re = 1;
	for each (Trustee^ var in this-> fixedParties)
	{
		re ^= var->GetPartyId();
	}
	return re;
}


String^  ThresholdSubset::ToString(){
	//todo: fix hack tostring from QS
	QualifiedSubset^ qs = gcnew QualifiedSubset();
	qs->Parties->AddRange(this->thresholdParties);

	QualifiedSubset^ qss = gcnew QualifiedSubset();
	qss->Parties->AddRange(this->fixedParties);
	return String::Format("{0} ^ Threshold({1},{2})[{3}]",qss->ToString(), K, N, qs->ToString());
}

IEnumerable<QualifiedSubset^>^ ThresholdSubset::GetQualifiedSubsets(){
	List<QualifiedSubset^>^ allpossibleQualified = this->permutation(K, this->thresholdParties);
	if (Enumerable::Count(fixedParties) > 0){
		for each (QualifiedSubset^ qs in allpossibleQualified)
		{
			qs->Parties->AddRange(fixedParties);
		}
	}
	return allpossibleQualified;
}

List<QualifiedSubset^>^ ThresholdSubset:: permutation(int k, IEnumerable<Trustee^>^ persons)
{
	 List<QualifiedSubset^>^ allpossibleQualified = gcnew List<QualifiedSubset^>();
	for (int j = 1; j < Enumerable::Count( persons); ++j)
	{
		QualifiedSubset^ qs = gcnew QualifiedSubset(swap(persons, k % (j + 1), j));
		allpossibleQualified->Add(qs);
		k = k / (j + 1);
	}
	return allpossibleQualified;
}

IEnumerable<Trustee^>^ ThresholdSubset::swap(IEnumerable<Trustee^>^ s, int i, int j)
{
	//
	// Swaps characters in a string. Must copy the characters and reallocate the string.
	//
	array<Trustee^>^ tr = Enumerable::ToArray(s); // Get characters
	Trustee^ temp = tr[i]; // Get temporary copy of character
	tr[i] = tr[j]; // Assign element
	tr[j] = temp; // Assign element
	return tr;
}

