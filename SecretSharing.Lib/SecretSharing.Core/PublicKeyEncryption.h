#pragma once
#include "stdafx.h"
#include "NTL\ZZ.h"
#include <vector>
#include <tuple>
#include "NativePtr.h"
using namespace System::Collections::Generic;
using namespace System;
using namespace SecretSharing::OptimalThreshold::Models;
using namespace NTL;
using namespace std;
namespace SecretSharingCore
{
	namespace Algorithms{
		namespace PKE{
			public ref class PublicKeyEncryption
			{
			public:
				PublicKeyEncryption();
				tuple<ZZ_p, ZZ_p> PublicKeyEncryption::GenerateKeyPair(ZZ q, ZZ_p G);
				Tuple<array<Byte>^, array<Byte>^>^  PublicKeyEncryption::GenerateKeyPair(array<Byte>^ q, array<Byte>^ G);
			};
		}
	}
}

