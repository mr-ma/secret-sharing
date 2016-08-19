#include "stdafx.h"
#include "PublicKeyEncryption.h"
#include "NTL\ZZ.h"
#include "NTL\ZZ_p.h"
#include <vector>
#include <tuple>
#include "NativePtr.h"
#include "NTLHelper.h"
using namespace System::Collections::Generic;
using namespace System;
using namespace SecretSharing::OptimalThreshold::Models;
using namespace NTL;
using namespace std;
namespace SecretSharingCore
{
	namespace Algorithms{
		namespace PKE{
			PublicKeyEncryption::PublicKeyEncryption()
			{

			}
			tuple<ZZ_p, ZZ_p> PublicKeyEncryption::GenerateKeyPair(ZZ q, ZZ_p G){
				tuple<ZZ_p, ZZ_p> keypair;
				// find a random x in multiplicative Zq 
				ZZ_p::init(q);
				ZZ_p x = random_ZZ_p();
				ZZ xz;
				conv(xz, x);
				//multiplicative elemets must be coprime with q-1 meaning GCD(x,1-1) =1
				while (!IsOne(GCD(xz, q-1)))
				{
					x = random_ZZ_p();
					conv(xz, x);
				}
				
				ZZ_p y = power(G, xz);
				keypair =  make_tuple(x, y);
				return keypair;
			}
			Tuple<array<Byte>^, array<Byte>^>^ PublicKeyEncryption::GenerateKeyPair(array<Byte>^ q, array<Byte>^ G){
				ZZ qz = NTLHelper::ByteToZZ(q);
				ZZ_p::init(qz);
				ZZ_p Gz = NTLHelper::ByteToZZ_p(G);
				tuple<ZZ_p, ZZ_p> pairs = this->GenerateKeyPair(qz, Gz);
				array<Byte>^ x =NTLHelper::ZZpToByte( get<0>(pairs));
				array<Byte>^ y = NTLHelper::ZZpToByte(get<1>(pairs));
				return gcnew Tuple<array<Byte>^, array<Byte>^>(x, y);
			}
		}
	}
}
