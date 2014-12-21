#pragma once
#include "stdafx.h"
#include "IShareCollection.h"
using namespace System::Collections::Generic;
using namespace System::Text;
	

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
		shares->Insert( index, share);
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
	ShareCollection::!ShareCollection(){
		for (int i = 0; i < shares->Count; i++)
		{
			IShare^ sh = shares[i];
			delete sh;
		}
		shares->Clear();
	}
	ShareCollection::~ShareCollection(){
		this->!ShareCollection();
	}


	String^ ShareCollection::ToString()
	{
		StringBuilder^ builder = gcnew StringBuilder();
		if (this->shares && this->shares->Count > 0){
			builder->AppendFormat("##Begin ShareCollection length={0}{1}", this->shares->Count, Environment::NewLine);
			for (int i = 0; i < this->shares->Count; i++)
			{
				builder->AppendFormat("index ={0} ,share= {1} {2}", i, shares[i]->ToString(), Environment::NewLine);
			}
			builder->AppendFormat("##End ShareCollection {0}", Environment::NewLine);
		}
		return builder->ToString();
	}
