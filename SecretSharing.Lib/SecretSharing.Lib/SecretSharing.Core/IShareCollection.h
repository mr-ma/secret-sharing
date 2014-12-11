#include "ISecretShare.h"
#pragma once

using namespace System::Collections::Generic;
using namespace std;

		public interface class IShareCollection
		{
		public:
			int GetCount();
			//int GetX(int index);
			//long GetY(int index);
			//List<IShare^>^ GetShare(int index);
			//void SetShare(int index, IShare^ share);
			//static void ScatterShareIntoCollection(List<IShare^>^ shares, List<ShareCollection^>^ currentCollection, int index);
			//List<IShare^>^ GatherShareFromCollection(List<ShareCollection^>^ currentCollection, int i);
		};


		public ref class ShareCollection :IShareCollection{
		public:
			virtual int GetCount();
			//virtual int ShareCollection::GetX(int index);
			//virtual long ShareCollection::GetY(int index);
			//virtual List<IShare^>^ GetShare(int index);
			//virtual void SetShare(int index, IShare^ share);
			//static  void ScatterShareIntoCollection(List<IShare^>^ shares, List<ShareCollection^>^ currentCollection, int index);
			//static List<IShare^>^ GatherShareFromCollection(List<ShareCollection^>^ currentCollection, int i);
		};

