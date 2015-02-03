// HelloWorld.cpp : main project file.
//#define _CRTDBG_MAP_ALLOC
//#include <stdlib.h>
//#include <crtdbg.h>
#include "stdafx.h"
#include <NTL/ZZ_pXFactoring.h>
#include "stdafx.h"
#include "Shamir.h"
#include "string.h"
#include "ShamirShare.h"
#include "BenalohLeichter.h"
#include "AccessStructure.h"
#include "Trustee.h"
#include "ISecretShare.h"
//#include "vld.h"
using namespace System;
using namespace std;
using namespace NTL;
using namespace System::Collections::Generic;
using namespace SecretSharingCore::Algorithms;
using namespace SecretSharingCore::Common;
using namespace SecretSharingCore::Algorithms::GeneralizedAccessStructure;

void MarshalString(String ^ s, string& os)
{
	using namespace Runtime::InteropServices;
	const char* chars =
		(const char*)(Marshal::StringToHGlobalAnsi(s)).ToPointer();
	os = chars;
	Marshal::FreeHGlobal(IntPtr((void*)chars));
}


void runByteChunkShare(){
	int k = 3;
	int n = 10;
	Byte chunkSize = 16;
	String^ secret = "1234567812345678";
	array<Byte>^ bytes = Encoding::UTF8->GetBytes(secret->ToCharArray());
	Shamir^ secretshare = gcnew Shamir();
	Nullable<double> a = NULL;
	List<IShareCollection^>^ shares = secretshare->DivideSecret(k, n, bytes,chunkSize,a);
	//List<IShareCollection^>^ sharesStr = secretshare->DivideSecret(k, n,secret);
	delete bytes;
	for (int i = 0; i < k; i++)
	{
		IShareCollection^ col = shares[i];
		//IShareCollection^ colstr = sharesStr[i];

		Console::WriteLine(col->ToString());
		//Console::WriteLine(colstr->ToString()); 

	/*	for (int j = 0; j < col->GetCount(); j++)
		{
			IShare^ share = col->GetShare(j);
			IShare^ shareStr = colstr->GetShare(j);
			Console::WriteLine(share->ToString());
			Console::WriteLine(shareStr->ToString());
		}*/
	}

	List<IShareCollection^>^ recshares = shares->GetRange(0, k);
	array<Byte>^ recoveredSecret = secretshare->ReconstructSecret(recshares, chunkSize);
	//List<IShareCollection^>^ recsharesstr = sharesStr->GetRange(0, k);
	//String^ recoveredSecretstr = secretshare->ReconstructSecret(recsharesstr);

	Console::WriteLine("Secret:"+Encoding::UTF8->GetString(recoveredSecret));
	for (int i = 0; i < recshares->Count; i++)
	{
		delete recshares[i];
	}
	//Console::WriteLine("SecretStr:" + recoveredSecretstr);
}


void PrintIShares(List<IShare^>^ shares){
	for (int j = 0; j < shares-> Count; j++)
	{
		Console::WriteLine(shares[j]->ToString());
	}
}

int main(array<System::String ^> ^args)
{
	//_CrtSetReportMode(_CRT_ERROR, _CRTDBG_MODE_DEBUG);

	/*Vec<ZZ_p> y = vec_ZZ_p();
	Vec<ZZ_p> x = vec_ZZ_p();

	ZZ_p::init(ZZ(199));
	ZZ_p y1 = ZZ_p(68);
	ZZ_p y2 = ZZ_p(2);
	ZZ_p y3 = ZZ_p(92);



		x.append(ZZ_p(1));
		x.append(ZZ_p(2));
		x.append(ZZ_p(3));


		y.append(y1);
		y.append(y2);
		y.append(y3);
		

	ZZ_pX interpolatedf = interpolate(x, y);
	cout << "interpol g(x):" << interpolatedf;*/

	//runByteChunkShare();

	//IShare^ sharezz = gcnew ShamirShare(1, &ZZ_p(2));

	/*
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
	Console::WriteLine("recovered secret with k shares:{0} secret:{1}", recshares->Count, recoveredSecret);*/

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
	*/	//_CrtDumpMemoryLeaks(); 


Trustee^ personA = gcnew Trustee(1);
Trustee^ personB = gcnew Trustee(2);
Trustee^ personC = gcnew Trustee(3);
Trustee^ personD = gcnew Trustee(3);

QualifiedSubset^ set = gcnew QualifiedSubset();
set->Parties = gcnew List<Trustee^>();
set->Parties->Add(personA);
set->Parties->Add(personB);
set->Parties->Add(personC);
set->Parties->Add(personD);

AccessStructure^ access = gcnew AccessStructure();
access->Accesses = gcnew List<QualifiedSubset^>();
access->Accesses->Add(set);


BenalohLeichter^ ben = gcnew BenalohLeichter();
	String^ secret = "1234567812345678";
	array<Byte>^ bytes = Encoding::UTF8->GetBytes(secret->ToCharArray());

	List<IShare^>^ shares=  ben->DivideSecret(bytes, access);
	PrintIShares(shares);

	array<Byte>^ reconbytes = ben->ReconstructSecret(shares);
	Console::WriteLine("Secret:" + Encoding::UTF8->GetString(reconbytes));

	Console::Read();
}
