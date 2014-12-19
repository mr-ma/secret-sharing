#pragma once
#include "stdafx.h"
using namespace SecretSharingCore::Common;
using namespace System::Text;
namespace SecretSharingCore
{
	namespace Common
	{
		public ref class ShamirShare :IShare
		{
		private:
			int _x;
			unsigned long _y;
			ZZ_p* _zz;
			ZZ* _prime;
		public:
			ShamirShare(const ShamirShare% rhs){
				_x = rhs._x;
				_y = rhs._y;
			}
			const ShamirShare operator=(const ShamirShare% rhs){
				_x = rhs._x;
				_y = rhs._y;
				return *this;
			}

			ShamirShare::ShamirShare(int x, unsigned long y){
				_x = x;
				_y = y;
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
			
			virtual unsigned long GetY(){
				return _y;
			}

			virtual String^ ToString() override
			{
				long a;
				conv(a, *_zz);
				StringBuilder^ builder = gcnew StringBuilder();
				builder->AppendFormat("X:{0} y:{1}", GetX(),a);
				return builder->ToString();
			}
			ShamirShare::~ShamirShare(){
				_zz = NULL;
				delete _zz;
			}
		internal:
			ZZ_p* GetZZ(){
				return _zz;
			}

			ZZ* GetPrime(){
				return _prime;
			}
		};
	}
}