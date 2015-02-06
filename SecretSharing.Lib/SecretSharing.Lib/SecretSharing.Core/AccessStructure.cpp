#pragma once
#include "stdafx.h"
#include "AccessStructure.h"
#include "QualifiedSubset.h"
#include <cliext/algorithm>
using namespace System;
using namespace System::Collections::Generic;
using namespace std;
using namespace SecretSharingCore::Common;
using namespace SecretSharingCore::Algorithms::GeneralizedAccessStructure;
using namespace System::Linq;

AccessStructure::AccessStructure(){
	this->Accesses = gcnew List<QualifiedSubset^>();
}

AccessStructure::AccessStructure(String^ minimalPath){
	this->Accesses = gcnew List<QualifiedSubset^>();
	try{
		array<String^>^ qualifiedsubsets = minimalPath->Split(',');
		for each (String^ qs in qualifiedsubsets)
		{
			QualifiedSubset^ qualifiedssObj = gcnew QualifiedSubset(qs);
			this->Accesses->Add(qualifiedssObj);
		}
	}
	catch(Exception^ ex) {
		throw gcnew System::Exception("Invalid access structure example of valid access: 1^2,3^2,2^3^4,2^5^6");
	}
}


List<Trustee^>^ AccessStructure::GetAllParties(){
	List<Trustee^>^ parties = gcnew List<Trustee^>();
	for each (QualifiedSubset^ qs in this->Accesses)
	{
		parties->AddRange(qs->Parties);
	}
	
	return System::Linq::Enumerable::ToList( System::Linq::Enumerable::Distinct(parties));
}

