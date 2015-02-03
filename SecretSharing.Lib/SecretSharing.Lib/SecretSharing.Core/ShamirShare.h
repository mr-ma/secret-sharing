#pragma once
#include "stdafx.h"
#include "NativePtr.h"
#include "NTL\ZZ.h"
#include "NTL\ZZ_p.h"
#include <vector>
#include "ISecretShare.h"
using namespace SecretSharingCore::Common;
using namespace System::Text;
using namespace NTL;
using namespace System;

namespace SecretSharingCore
{
	namespace Common
	{
		public ref class ShamirShare :IShare
		{
		private:
			int _x;
			CAutoNativePtr<ZZ_p> _zz;
			CAutoNativePtr<ZZ> _prime;
		public:
			ShamirShare(int x, array<Byte>^ y, array<Byte>^ p){
				this->_x = x;
				pin_ptr<unsigned char> unmanagedY = &y[0];
				ZZ _y = ZZFromBytes(unmanagedY, y->Length);
				this->_zz = new ZZ_p(to_ZZ_p(_y));

				pin_ptr<unsigned char> unmanagedP = &p[0];
				ZZ _p = ZZFromBytes(unmanagedP, p->Length);
				this->_prime = new ZZ(_p);
			}
			ShamirShare(const ShamirShare% rhs){
				_x = rhs._x;
				//_prime =rhs._prime;
				//_zz = rhs._zz;
			}
			const ShamirShare operator=(const ShamirShare% rhs){
				_x = rhs._x;
				return *this;
			}

			ShamirShare::ShamirShare(int x, unsigned long y){
				_x = x;
			}
			ShamirShare::ShamirShare(int x, ZZ_p* y,ZZ* prime)
			{
				_x = x;
				_zz = y;
				_prime = prime;
			}

			virtual int GetX(){
				return _x;
			}
			
			virtual array<Byte>^ GetY(){
				ZZ share;
				conv(share, *_zz);
				array<Byte>^ shareArray = GetArrayOfZZ(share);
				return shareArray;
			}
			int GetYSize(){
				ZZ share;
				conv(share, *_zz);
				return share.MaxAlloc();
			}
			virtual array<Byte>^ GetP(){
				array<Byte>^ pArray = GetArrayOfZZ(*_prime);
				return pArray;
			}

			array<Byte>^ GetArrayOfZZ(ZZ number){
				unsigned char* unmanagedArray = (unsigned char*)malloc(sizeof(unsigned char) * number.MaxAlloc());
				BytesFromZZ(unmanagedArray, number, number.MaxAlloc());
				array<Byte>^ _Data = gcnew array<Byte>(number.MaxAlloc());
				System::Runtime::InteropServices::Marshal::Copy(IntPtr((void *)unmanagedArray), _Data, 0, number.MaxAlloc());
				delete unmanagedArray;
				return _Data;
			}

			virtual String^ ToString() override
			{
				String^ strPrime = Convert::ToBase64String(GetP());
				String^ strShare = Convert::ToBase64String(GetY());
				StringBuilder^ builder = gcnew StringBuilder();
				builder->AppendFormat("X:{0} y:{1} ySize:{2} yarraySize:{3} p:{4}", GetX(), strPrime, (*_prime).MaxAlloc(),
					this->GetY().Length,strShare);
				return builder->ToString();
			}
		
			ShamirShare::!ShamirShare(){
				///native pointer takes care of releasing pointers
			}
		internal:
			ZZ_p* GetZZ(){
				return _zz;
			}

			ZZ* GetPrime(){
				return _prime;
			}
		
		protected:
			ShamirShare::~ShamirShare(){
				this->!ShamirShare();
			}
		};
	}
}