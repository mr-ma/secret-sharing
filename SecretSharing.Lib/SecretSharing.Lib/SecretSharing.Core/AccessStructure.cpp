#pragma once
#include "stdafx.h"
#include "AccessStructure.h"
#include "QualifiedSubset.h"
using namespace System;
using namespace System::Collections::Generic;
using namespace std;
using namespace SecretSharingCore::Common;
using namespace SecretSharingCore::Algorithms::GeneralizedAccessStructure;

AccessStructure::AccessStructure(){
	this->Accesses = gcnew List<QualifiedSubset^>();
}
