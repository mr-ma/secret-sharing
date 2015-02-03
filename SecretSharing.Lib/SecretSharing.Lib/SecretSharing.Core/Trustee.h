#pragma once
#include "stdafx.h"
#include "IShareCollection.h"
using namespace System;
using namespace System::Collections::Generic;
using namespace std;
using namespace SecretSharingCore::Common;

namespace SecretSharingCore
{
	namespace Algorithms
	{
		namespace GeneralizedAccessStructure{
			public ref class Trustee{
			private :
				int partyId;
			public:
				List<IShare^>^ Shares;

				Trustee::Trustee(int id);
				int GetPartyId();
				void Trustee::AddShare(IShare^ share);
				virtual bool Equals(Object^ o) override;
				virtual int GetHashCode() override;
				
			};
		}
	}
}