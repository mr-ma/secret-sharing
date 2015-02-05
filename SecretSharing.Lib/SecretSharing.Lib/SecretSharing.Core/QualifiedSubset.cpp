#pragma once
#include "stdafx.h"
#include "QualifiedSubset.h"
using namespace System;
using namespace System::Collections::Generic;
using namespace std;
using namespace SecretSharingCore::Common;
using namespace SecretSharingCore::Algorithms::GeneralizedAccessStructure;

QualifiedSubset::QualifiedSubset(){
	this->Parties = gcnew List<Trustee^>();
}

bool QualifiedSubset::Equals(Object^ obj) { // no "override" here 
	if (obj == nullptr || GetType() != obj->GetType())
		return false;

	QualifiedSubset^ p = dynamic_cast<QualifiedSubset^>(obj);
	if (this->Parties->Count == p->Parties->Count){
		
		for each (Trustee^ var in p->Parties)
		{
			if (!this->Parties->Contains(var))
				return false;
		}
		return true;
	}
	return false;
}
int QualifiedSubset::GetHashCode() { // no "override" here
	int re = 1;
	for each (Trustee^ var in this->Parties)
	{
		re ^= var->GetPartyId();
	}
	return re;
}

void QualifiedSubset::AssignSecretPercentage(){
	for each (Trustee^ var in Parties)
	{
		var->SecretSharePercentage = 1/Parties->Count;
	}
}
