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
			ShamirShare::ShamirShare(int x, ZZ_p* y)
			{
				_x = x;
				_zz = y;
			}

			virtual int GetX(){
				return _x;
			}
			
			virtual unsigned long GetY(){
				return _y;
			}
			virtual String^ ToString() override
			{
				StringBuilder^ builder = gcnew StringBuilder();
				builder->AppendFormat("X:{0}", GetX());
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
		};
	}
}