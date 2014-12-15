
#ifndef NTL_LazyTable__H
#define NTL_LazyTable__H

#include <NTL/tools.h>
#include <NTL/SmartPtr.h>
#include <NTL/thread.h>

NTL_OPEN_NNS


/***************************************************************************


LazyTable<T,MAX>: template class for lazy initialization of objects whose
values do not change after initialization.
In a multi-threaded environment, this makes use of "double checked locking"
for an efficient, thread-safe solution.

Usage:

   LazyTable<T,MAX> tab; // declaration of the lazy table, with max size == MAX

    ...

   do {
      LazyTable<T,MAX>::Builder builder(tab, n); // request length n
      long amt = builder.amt();
      if (!amt) break;      

      ... initialize elements i = n-amt..n-1 via builder.access(i)
                             
   } while(0);               // When this scope closes, 
                             // the table is fully initialized to length n


   T val = table[i];         // read-only access to table elements 0..n-1
                             

It is important to follow this recipe carefully.  In particular,
the builder must be enclosed in a scope, as it's destructor
plays a crucial role in finalizing the initialization.


template<class T, long MAX>
class LazyTable {
public:
   LazyTable();

   LazyTable(const LazyTable&);             // "deep" copies
   LazyTable& operator=(const LazyTable&);

   const T& operator[] (long i) const;

   ~LazyTable();

   long length() const; 

   kill();  // destroy and reset

   class Builder {
      Builder(const LazyTable&, long request); 
     ~Builder()

      T& access (long i);
      long amt() const;

   };
   


****************************************************************************/

// FIXME: Right now, the builder mechanism is not exception safe.
// Although the destructor will release the mutex (if any), it
// mark the object as "constructed", even if it is not.
// If we want exception safety, we may have to modify the
// interface, so that we invoke builder.finalize(k) to 
// signal (to the destructor) that builder[k] has been built.
// Care would have to be taken to ensure that we "roll back"
// to the right internal state in this case.


// NOTE: For more on double-checked locking, see
// http://preshing.com/20130930/double-checked-locking-is-fixed-in-cpp11/


template<class T, long MAX>
class LazyTable {
private:
   mutable AtomicLong len; 
   mutable MutexProxy mtx;

   mutable UniqueArray<T> data;

public:
   LazyTable() : len(0) { }

   void kill() 
   { 
      data.reset();
      len = 0; 
   }

   LazyTable& operator=(const LazyTable& other) 
   {
      if (this == &other) return *this;

      long olen = other.len;

      if (olen == 0) {
         data.reset();
      }
      else {
         if (!data) data.make(MAX);

         long i;
         for (i = 0; i < olen; i++) data[i] = other.data[i];
      }

      len = olen;

      return *this;
   }
   
   LazyTable(const LazyTable& other) : len(0)
   {
      *this = other;
   }

   const T& operator[] (long i) const 
   { 
      // FIXME: add optional range checking

      return data[i]; 
   }

   long length() const { return len; }

   class Builder {
   private:
      const LazyTable& ref;
      long request;
      GuardProxy guard;

      long amount;
      T *p;

      Builder(const Builder&); // disabled
      void operator=(const Builder&); // disabled

   public:
      Builder(const LazyTable& _ref, long _request) 
      : ref(_ref), request(_request), guard(_ref.mtx), amount(0), p(0)
      {
         if (request < 0 || request >= MAX) 
            NTL_NNS Error("request out of rangle in LazyTable::Builder"); 


         // Double-checked locking
         if (request <= ref.len || (guard.lock(), request <= ref.len)) 
            return;

         if (!ref.data) ref.data.make(MAX);
         amount = request - ref.len;
         p = ref.data.get();
      }

      ~Builder() { if (amount) ref.len = request; }

      T& access(long i) 
      {
         // allow read/write access to elements request-amount..request-1
         if (i < request-amount || i >= request)
            NTL_NNS Error("index out if bounds in LazyTable::Builder::access");

         return p[i];
      }

      long amt() const { return amount; }
   };
};


NTL_CLOSE_NNS

#endif
