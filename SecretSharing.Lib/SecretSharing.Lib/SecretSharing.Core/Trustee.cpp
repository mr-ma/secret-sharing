#pragma once
#include "stdafx.h"
#include "Trustee.h"
using namespace System;
using namespace System::Collections::Generic;
using namespace std;
using namespace SecretSharingCore::Common;
using namespace SecretSharingCore::Algorithms::GeneralizedAccessStructure;

Trustee::Trustee(int id){
	this->partyId = id;
}

bool Trustee::Equals(Object^ obj) { // no "override" here 
	if (obj == nullptr || GetType() != obj->GetType())
		return false;

	Trustee^ p = dynamic_cast<Trustee^>(obj);
	return (this->partyId == p->partyId);
}
int Trustee::GetHashCode() { // no "override" here
	return partyId;
}

int Trustee::GetPartyId(){
	return partyId;
}

void Trustee::AddShare(IShare^ share){
	//TODO: to be added
}