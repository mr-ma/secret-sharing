#pragma once
#include "stdafx.h"
#include "IShareCollection.h"
using namespace System::Collections::Generic;

	

	ShareCollection::ShareCollection(){
		shares = gcnew List<IShare^> ();
	}

	int ShareCollection::GetCount(){
		return shares->Count;
	}
	IShare^ ShareCollection::GetShare(int index){
		return shares[index];
	}
	void ShareCollection::SetShare(int index, IShare^ share){
		shares[index] = share;
	}

	 void ShareCollection::ScatterShareIntoCollection(List<IShare^>^ shares, List<IShareCollection^>^ currentCollection, int index)
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
	List<IShare^>^ ShareCollection::GatherShareFromCollection(List<IShareCollection^>^ currentCollection, int i)
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

