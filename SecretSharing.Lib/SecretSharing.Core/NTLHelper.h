#pragma once
#include "stdafx.h"
#include "NTL\ZZ.h"
#include "NTL\ZZ_p.h"
using namespace System;
using namespace NTL;
using namespace std;
namespace SecretSharingCore
{
	public  ref  class  NTLHelper
	{
	public:
		NTLHelper(){

		}
		static array<Byte>^ ZZToByte(ZZ number){
			long nrBytes = NumBytes(number);
			unsigned char* unmanagedArray = (unsigned char*)malloc(sizeof(unsigned char)*nrBytes);
			BytesFromZZ(unmanagedArray, number, nrBytes);
			array<Byte>^ _Data = gcnew array<Byte>(nrBytes);
			System::Runtime::InteropServices::Marshal::Copy(IntPtr((void *)unmanagedArray), _Data, 0, nrBytes);
			delete unmanagedArray;
			if(_Data->Length == 0) {
				_Data = gcnew array<Byte>(1);
				_Data[0] = 0;
			}
			return _Data;
		}
		static ZZ ByteToZZ(array<Byte>^ arr){
			if(arr->Length > 0){
			pin_ptr<unsigned char> unmanagedSecretArray = &arr[0];
			ZZ zz = ZZFromBytes(unmanagedSecretArray, arr->Length);
			return zz;}
			//for 0 the array is empty
			return ZZ(0);
		}

		static array<Byte>^ ZZpToByte(ZZ_p number){
			ZZ zzNr;
			conv(zzNr, number);
			return ZZToByte(zzNr);
		}
		static ZZ_p ByteToZZ_p(array<Byte>^ arr){
			ZZ zz_Nr = ByteToZZ(arr);
			ZZ_p zz_p_Nr;
			conv(zz_p_Nr, zz_Nr);
			return zz_p_Nr;
		}

		static array<Byte>^ NumberToZZByte(long number){
			ZZ nz = ZZ(number);
			return ZZToByte(nz);
		}
		static array<Byte>^ NumberToZZpByte(long number){
			ZZ_p nz = ZZ_p(number);
			return ZZpToByte(nz);
		}
		static void InitZZ_p(array<Byte>^ number){
			ZZ  p = ByteToZZ(number);
			ZZ_p::init(p);
		}
	};
}
