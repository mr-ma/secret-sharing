#include "stdafx.h"
#include "IShareCollection.h"
#include "ISecretShare.h"
#include <stddef.h>
using namespace System::Collections::Generic;
using namespace std;
/*
public ref class ShareCollection
{
private:
	List<IShare^>^ shares;

public:
	ShareCollection::ShareCollection(){
		shares = gcnew List<IShare^> ();
	}

	virtual int GetCount(){
		return shares->Count;
	}
	virtual IShare^ GetShare(int index){
		return shares[index];
	}
	virtual void SetShare(int index, IShare^ share){
		shares[index] = share;
	}

	static void ShareCollection::ScatterShareIntoCollection(List<IShare^>^ shares, List<ShareCollection^>^ currentCollection, int index)
	{
		if (currentCollection->Count == 0)
		{
			for (int i = 0; i < shares->Count; i++)
			{
				currentCollection->Add(gcnew ShareCollection());
			}
		}

		for (int j = 0; j < shares->Count; j++)
		{
			currentCollection[j]->SetShare(index ,shares[j]);
		}

	}
	static List<IShare^>^ ShareCollection::GatherShareFromCollection(List<ShareCollection^>^ currentCollection, int i)
	{
		List<IShare^>^ shares = gcnew List<IShare^>();
		for (int j = 0; j < currentCollection->Count; j++)
		{
			shares->Add(currentCollection[j]->GetShare(i));
		}
		return shares;
	}
	ShareCollection::~ShareCollection(){
		shares->Clear();
	}

};
*/