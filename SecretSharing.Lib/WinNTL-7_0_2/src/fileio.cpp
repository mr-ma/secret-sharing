
#include <NTL/fileio.h>
#include <NTL/thread.h>

#include <string>
#include <sstream>
#include <iomanip>
#include <ctime>


NTL_START_IMPL


void OpenWrite(ofstream& s, const char *name)
{
   s.open(name, ios::out);

   if (!s) {
      cerr << "open write error: " << name;
      Error("");
   }
}


void OpenRead(ifstream& s, const char *name)
{
   s.open(name, ios::in);
   if (!s) {
      cerr << "open read error: " << name;
      Error("");
   }
}


// FIXME: The FileName function incorporate thread ID, so it should
// generate unique prefixes across threads within a process.  
// But it doesn't guarantee uniqueness across processes.  
// It would be best to incorporate process ID, but there is no good, 
// cross platform way to get that.
// For now, I mangle the current time with some low-order digits
// from GetTime and use that as a proxy.




const char *FileName(const char* stem, long d)
{
   NTL_THREAD_LOCAL static string sbuf;
   NTL_THREAD_LOCAL static unsigned long first_time;
   NTL_THREAD_LOCAL static bool first_time_init = false;

   if (!first_time_init) {
      double t = GetTime();
      unsigned long t1 = (unsigned long) (long( (t - long(t)) * 10000L ));
      first_time = ((unsigned long) time(0))*10000UL + t1;
      first_time_init = true;
   }

   stringstream ss;
   ss << "tmp-ntl" << first_time << "-" << CurrentThreadID() << stem;
   ss << "-" << setfill('0') << setw(5) << d;
   sbuf = ss.str();
   return sbuf.c_str();
}

NTL_END_IMPL
