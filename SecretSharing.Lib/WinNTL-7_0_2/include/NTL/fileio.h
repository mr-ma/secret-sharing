
#ifndef NTL_fileio__H
#define NTL_fileio__H

#include <NTL/tools.h>
#include <fstream>                                                              


NTL_OPEN_NNS


void OpenWrite(NTL_SNS ofstream& s, const char *name);

// opens file for writing...aborts if fails

void OpenRead(NTL_SNS ifstream& s, const char *name);

// opens file for reading


const char *FileName(const char* stem, long d);

// builds the name from stem-DDDDD, returns pointer to buffer

NTL_CLOSE_NNS

#endif


