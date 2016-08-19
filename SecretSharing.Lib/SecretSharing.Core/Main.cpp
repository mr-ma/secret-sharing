// HelloWorld.cpp : main project file.
//#define _CRTDBG_MAP_ALLOC
//#include <stdlib.h>
//#include <crtdbg.h>
#include "stdafx.h"
#include <NTL/ZZ_pXFactoring.h>
#include "Shamir.h"
#include "string.h"
#include "ShamirShare.h"
#include "BenalohLeichter.h"
//#include "AccessStructure.h"
//#include "Trustee.h"
#include "ISecretShare.h"
#include "NonInteractiveChaumPedersen.h"
#include "NTLHelper.h"
#include "PrimeGenerator.h"
#include <vector>
#include "Schoenmakers.h"
#include "PublicKeyEncryption.h"
#include <tuple>
//#include "vld.h"
using namespace System;
using namespace std;
using namespace NTL;
using namespace System::Collections::Generic;
using namespace SecretSharingCore::Algorithms;
using namespace SecretSharingCore::Algorithms::PVSS;
using namespace SecretSharingCore::Algorithms::PKE;
using namespace SecretSharingCore::Common;
using namespace SecretSharingCore::Algorithms::GeneralizedAccessStructure;
using namespace SecretSharing::OptimalThreshold::Models;
using namespace SecretSharing::OptimalThreshold;
using namespace SecretSharingCore::ZKProtocols;
using namespace SecretSharingCore;

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
#ifdef calcPrimeTime
	double a = 0;
	List<IShareCollection^>^ shares = secretshare->DivideSecret(k, n, bytes,chunkSize,a);
#else
	List<IShareCollection^>^ shares = secretshare->DivideSecret(k, n, bytes, chunkSize);
#endif
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
	//PrimeGenerator^ pg = gcnew PrimeGenerator();
	//ZZ p = ZZ(263);
	//ZZ_p::init(p);
	//cout<< pg->IsGeneratorOfP(p, ZZ_p(5));
	//Console::Read();

	Schoenmakers^ sch = gcnew Schoenmakers();
	sch->SelectPrimeAndGenerators(1);
	ZZ_p g = sch->Getg();
	ZZ_p G = sch->GetG();
	ZZ q = sch->Getq();
	cout <<"g:"<<g <<'\n';
	cout << "G:" << G << '\n';
	cout << "q:" << q << '\n';
	
	int n = 5;
	// generate n keypairs
	vector<ZZ_p> publickeys;
	vector<tuple<ZZ_p, ZZ_p>> keypairs;
	PublicKeyEncryption^ pke = gcnew PublicKeyEncryption();
	for (int i = 0; i < n; i++)
	{
		tuple<ZZ_p,ZZ_p> pair= pke->GenerateKeyPair(q, G);
		keypairs.push_back(pair);
		//cout <<"x:"<< get<0>(pair)<<" y:"<<get<1>(pair)<<'\n';
		publickeys.push_back(get<1>(pair));
	}
	

	int t = 3;
	
	vector<ZZ_p> encryptedShares;
	vector<ZZ_p> commitments;
	vector<ZZ_p> c,r;
	ZZ_p secret;
	//provide public keys to schoenmakers
	sch->SetPublicKeys(publickeys);

	List<array<Byte>^>^ commitsList = gcnew List<array<Byte>^>();
	array<Byte>^ secretB;
	array<Byte>^ U;
	array<Byte>^ sigma = Encoding::UTF8->GetBytes("4");
	List<SchoenmakersShare^>^ shares =  sch->Distribute(t, n,sigma, commitsList, secretB,U);
	List<SchoenmakersShare^>^ poolesshares = gcnew List<SchoenmakersShare^> ();
	int i = 0;
	for each (SchoenmakersShare^ share in shares)
	{
		SchoenmakersShare^ pooledshare = share;
		sch->PoolShare(gcnew Tuple<array<Byte>^, array<Byte>^>(NTLHelper::ZZpToByte(get<0>(keypairs.at(i))), NTLHelper::ZZpToByte(get<1>(keypairs.at(i)))), pooledshare);
		poolesshares->Add(pooledshare);
		i++;
	}
	array<Byte>^ reconed = sch->Reconstruct(t, poolesshares, U);
	//Console::Read();

	sch->Distribute(t, n, encryptedShares, commitments, r, c,secret);

	for (int i = 0; i < n; i++)
	{
		bool verified = sch->VerifyDistributedShare(i+1, r.at(i), c.at(i), encryptedShares.at(i), commitments, publickeys.at(i));
		cout << "share i:" << i+1 << " verified status:" << verified<<'\n';
		//if (!verified) throw gcnew Exception("Failed to verify the share!");
	}
	vector<ZZ_p> rshares;
	vector<ZZ_p> cshares;
	vector<ZZ_p> S;
	for (int i = 0; i < encryptedShares.size(); i++)
	{
		ZZ_p rshare, cshare;
		//each party pool his share and compute zero knowledge r and c
		ZZ_p Si = sch->PoolShare(get<0>(keypairs.at(i)), get<1>(keypairs.at(i)), encryptedShares.at(i), rshare, cshare);
		rshares.push_back(rshare);
		cshares.push_back(cshare);
		S.push_back(Si);
	}
	//dealer, at this point can verify decrypted shares
	bool verifyConstruction = sch->VerifyPooledShares(t, S, encryptedShares, rshares, cshares);
	cout << "Construction verified:" << verifyConstruction<<'\n';

	if (verifyConstruction){
		ZZ_p secret = sch->Reconstruct(t,S);
		cout <<"secret:" <<secret<<'\n';
	}
	Console::Read();
	return 0;

	again:
	array<Byte>^ Prime =   NTLHelper::NumberToZZByte(17);
	NTLHelper::InitZZ_p(Prime);

	array<Byte>^ Base1 = NTLHelper::NumberToZZpByte(3);
	array<Byte>^ Base2 = NTLHelper::NumberToZZpByte(5); 
    array<Byte>^ Result1 = NTLHelper::NumberToZZpByte((long)pow(3,7));
	array<Byte>^ Result2 = NTLHelper::NumberToZZpByte((long)pow(5,7));
	array<Byte>^ Secret = NTLHelper::NumberToZZpByte(7);
	array<Byte>^ R;
	array<Byte>^ C;

	
	
	NonInteractiveChaumPedersen^ NICP = gcnew NonInteractiveChaumPedersen(Prime);
	NICP->ComputeProofs(Base1,Base2,Result1,Result2,Secret,R,C);
	bool Proved = NICP ->VerifyProofs(Base1,Base2,Result1,Result2,R,C);
	cout << "Proved:" << Proved<<'\n';
	Console::Read();
	goto again;

	/*ZZ prime = ZZ(17);
	ZZ_p::init(prime);
	NonInteractiveChaumPedersen^ nicp = gcnew NonInteractiveChaumPedersen(prime);
	ZZ_p base1 = ZZ_p(3);
	ZZ_p base2 = ZZ_p(5);
	ZZ secret = ZZ(7);
	ZZ_p result1 = power(base1, secret);
	ZZ_p result2 = power(base2, secret);
	ZZ_p c;
	ZZ_p r;
	nicp->ComputeProofs(base1, base2, result1, result2, to_ZZ_p(secret), r, c);

	cout <<"r: "<< r<<'\n';
	cout << "c: " << c << '\n';

	bool proved = nicp->VerifyProofs(base1, base2, result1, result2, r, c);
	cout << "proved:" << proved<<'\n';
	Console::Read();

	goto again;*/

	/*BenalohLeichter^ benaloh = gcnew BenalohLeichter();
	AccessStructure^ access = gcnew AccessStructure("p1^p2^p3,p2^p3^p4,p1^p3^p4,p1^p2^p4");
	access = ThresholdHelper::OptimiseAccessStructure(access, true);
	array<Byte>^ secretBytes = Encoding::UTF8->GetBytes("12345678");
	List<IShareCollection^>^ shares =  benaloh->DivideSecret(secretBytes, access);
	array<Byte>^ reconSecretBytes = benaloh->ReconstructSecret(shares[0]);
	Console::WriteLine(Encoding::UTF8->GetString(reconSecretBytes));
*/
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
}
