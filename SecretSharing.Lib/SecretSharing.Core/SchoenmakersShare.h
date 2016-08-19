#pragma once
#include "NTL\ZZ.h"
#include "NTL\ZZ_p.h"
#include "NativePtr.h"
using namespace std;
using namespace NTL;
using namespace System;
namespace SecretSharingCore
{
	namespace Algorithms{
		namespace PVSS{
			public ref class SchoenmakersShare
			{
			
			public:
				CAutoNativePtr<ZZ_p> c;
				CAutoNativePtr<ZZ_p> r;
				CAutoNativePtr<ZZ_p> Y;
				CAutoNativePtr<ZZ_p> S;
				CAutoNativePtr<ZZ_p> proofc;
				CAutoNativePtr<ZZ_p> proofr;
				
					SchoenmakersShare::SchoenmakersShare( ZZ_p* Y,ZZ_p* c, ZZ_p* r);
				bool SchoenmakersShare::IsPooled();
			};
		}
	}
}

