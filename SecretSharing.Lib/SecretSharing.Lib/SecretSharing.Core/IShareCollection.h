#include "ISecretShare.h"
#pragma once

using namespace System::Collections::Generic;
using namespace std;
using namespace SecretSharingCore::Common;
namespace SecretSharingCore
{
	namespace Common
	{
		public interface class IShareCollection
		{
		public:
			int GetCount();
			//int GetX(int index);
			//long GetY(int index);
			IShare^ GetShare(int index);
			void SetShare(int index, IShare^ share);
			String^ ToString();
			//static void ScatterShareIntoCollection(List<IShare^>^ shares, List<ShareCollection^>^ currentCollection, int index);
			//List<IShare^>^ GatherShareFromCollection(List<ShareCollection^>^ currentCollection, int i);
		};


		public ref class ShareCollection :IShareCollection{
		private:

		public:
			List<IShare^>^ shares;
			virtual int ShareCollection::GetCount();
			//virtual int ShareCollection::GetX(int index);
			//virtual long ShareCollection::GetY(int index);
			virtual IShare^ ShareCollection::GetShare(int index);
			virtual void ShareCollection::SetShare(int index, IShare^ share);
			static  void ScatterShareIntoCollection(List<IShare^>^ shares, List<IShareCollection^>^ currentCollection, int index);
			static List<IShare^>^ GatherShareFromCollection(List<IShareCollection^>^ currentCollection, int i);
			virtual String^ ToString() override;
			ShareCollection::ShareCollection();
			ShareCollection::~ShareCollection();
		};
	}
}