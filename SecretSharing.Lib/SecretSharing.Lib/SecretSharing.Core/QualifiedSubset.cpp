#pragma once
#include "stdafx.h"
#include "QualifiedSubset.h"
using namespace System;
using namespace System::Collections::Generic;
using namespace std;
using namespace SecretSharingCore::Common;
using namespace SecretSharingCore::Algorithms::GeneralizedAccessStructure;
using namespace System::Linq;

QualifiedSubset::QualifiedSubset(){
	this->Parties = gcnew List<Trustee^>();
}
QualifiedSubset::QualifiedSubset(String^ subset){
	this->Parties = gcnew List<Trustee^>();
	try{
		array<String^>^ parties = subset->Split('^');
		for each (String^ p in parties)
		{
			Trustee^ partyObj = gcnew Trustee(p);
			this->Parties->Add(partyObj);
		}
	}
	catch (Exception^ ex) {
		throw gcnew System::Exception("Invalid Qualified subset example of valid subset: 1^2^3");
	}
}
QualifiedSubset::QualifiedSubset(IEnumerable<Trustee^>^ qualifiedPath)
{
	this->Parties = gcnew List<Trustee^>();
	this->Parties->AddRange(qualifiedPath);
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


String^  QualifiedSubset::ToString(){
	IEnumerable<String^>^ stringified = Enumerable::Select(this->Parties, gcnew Func<Trustee^, String^>(&Trustee::ToString));
	Func<String^, String^, String^>^ func = gcnew Func < String^, String^, String^ >(this, &QualifiedSubset::aggreagate);
	if (Enumerable::Count(stringified) > 0){
		return Enumerable::Aggregate(stringified, func);
	}
	else{
		return "";
	}
}

String^ QualifiedSubset::aggreagate(String^ current, String^ next){
	return current + "," + next;
}