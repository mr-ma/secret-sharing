// HelloWorld.cpp : main project file.

#include "stdafx.h"
#include <NTL/ZZ_pXFactoring.h>
#include "stdafx.h"
#include "Shamir.h"
#include "string.h"

using namespace System;
using namespace std;
using namespace NTL;
using namespace System::Collections::Generic;
using namespace SecretSharingCore::Algorithms;


void MarshalString(String ^ s, string& os)
{
	using namespace Runtime::InteropServices;
	const char* chars =
		(const char*)(Marshal::StringToHGlobalAnsi(s)).ToPointer();
	os = chars;
	Marshal::FreeHGlobal(IntPtr((void*)chars));
}
int main(array<System::String ^> ^args)
{
	
	int k = 4;
	int n = 10;
	String^ secret = "1234";

	Shamir^ secretshare = gcnew Shamir();
	List<IShareCollection^>^ shares = secretshare->DivideSecret(k, n, secret);
	
	
	for (int i = 0; i < shares->Count; i++)
	{
		IShareCollection^ col = shares[i];
		Console::WriteLine(col->ToString());
		for (int j = 0; j < col->GetCount(); j++)
		{
			IShare^ share = col->GetShare(j);
			Console::WriteLine(share->ToString());
		}
	}


	List<IShareCollection^>^ frecshares = shares->GetRange(0, k - 1);
	String^ frecoveredSecret = secretshare ->ReconstructSecret(frecshares);
	Console::WriteLine("recovered secret with k-1 shares:{0} secret:{1}", frecshares->Count, frecoveredSecret);


	List<IShareCollection^>^ recshares = shares->GetRange(0, k);
	String^ recoveredSecret = secretshare->ReconstructSecret(recshares);
	Console::WriteLine("recovered secret with k shares:{0} secret:{1}", recshares->Count, recoveredSecret);

	/*
	int a1 = 166;
	int a2 = 94;

	int p = 1613;

	ZZ_p::init(ZZ(p));

	ZZ_pX f;
	f.SetLength( k-1);
	SetCoeff(f, 0, ZZ_p(secret));
	SetCoeff(f, 1, ZZ_p(a1));
	SetCoeff(f, 2, ZZ_p(a2));

	cout << f;

	ZZ_p d1 = eval(f, ZZ_p(1));
	ZZ_p d2 = eval(f, ZZ_p(2));
	ZZ_p d3 = eval(f, ZZ_p(3));
	ZZ_p d4 = eval(f, ZZ_p(4));
	ZZ_p d5 = eval(f, ZZ_p(5));
	ZZ_p d6 = eval(f, ZZ_p(6));

	cout << '\n' << d1 << '\n' << d2 << '\n' << d3 << '\n' << d4 << '\n' << d5 << '\n' << d6;

	Vec<ZZ_p> x = vec_ZZ_p();
	for (int i = 1; i < 4; i++)
	{
		x.append(ZZ_p(i));
	}
	
	Vec<ZZ_p> y = vec_ZZ_p();
	y.append(d1);
	y.append(d2);
	y.append(d3);
	ZZ_pX interpolatedf = interpolate(x, y);
	cout << "secret is:" << eval(interpolatedf, ZZ_p(0));

	/*ZZ p;
	GenPrime(p, 10);
	long val = RandomPrime_long(8);
	cout << p << "/n";
	cout << val;
	cin >> p;
	ZZ_p::init(p);

	

	ZZ_pX f;
	cin >> f;

	Vec< Pair< ZZ_pX, long > > factors;

	CanZass(factors, f);  // calls "Cantor/Zassenhaus" algorithm

	cout << factors << "\n";
	*/
	Console::Read();
}
