#pragma once
#include "NTL\ZZ.h"
#include "NativePtr.h"
using namespace NTL;
using namespace System;
namespace SecretSharingCore
{
	namespace ZKProtocols{
		public ref class NonInteractiveChaumPedersen
		{
		private:
			 CAutoNativePtr<ZZ> q; 
		public:
			NonInteractiveChaumPedersen::NonInteractiveChaumPedersen(array<Byte>^ PrimeFieldInBytes);
			NonInteractiveChaumPedersen::NonInteractiveChaumPedersen(ZZ PrimeField);
			void NonInteractiveChaumPedersen::ComputeProofs(array<Byte>^ Base1, array<Byte>^ Base2, 
				array<Byte>^ Result1, array<Byte>^ Result2, array<Byte>^ Secret, array<Byte>^% r,array<Byte>^% c);

			bool NonInteractiveChaumPedersen::VerifyProofs(array<Byte>^ Base1, array<Byte>^ Base2, 
				array<Byte>^ Result1, array<Byte>^ Result2, array<Byte>^ r,array<Byte>^ c);

			void NonInteractiveChaumPedersen::ComputeProofs(ZZ_p Base1, ZZ_p Base2,
				ZZ_p Result1, ZZ_p Result2, ZZ_p Secret, ZZ_p% r, ZZ_p% c);

			bool NonInteractiveChaumPedersen::VerifyProofs(ZZ_p Base1, ZZ_p Base2,
				ZZ_p Result1, ZZ_p Result2, ZZ_p r, ZZ_p c);


			array<Byte>^ NonInteractiveChaumPedersen::ComputeHash(array<Byte>^ Result1, array<Byte>^ Result2,array<Byte>^ A1, array<Byte>^ A2);
		};
	}
}

