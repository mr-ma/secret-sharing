
/***************************************************************************


Lazy<T>: template class for lazy initialization of objects whose
values do not change after initialization.
In a multi-threaded environment, this makes use of "double checked locking"
for an efficient, thread-safe solution.

Usage:

   Lazy<T> obj; // declaration of the lazy object

    ...

   do {
      Lazy<T>::Builder builder(obj);
      if (!builder) break;   // if builder is non-null, it points to the underlying
      *builder = .... ;      // object, which has just been default-initialized.
                             // We can then complete the initialization process.
   } while(0);               // When this scope closes, the object is fully initialized.
                             // subsequent attempts to build the object will yield
                             // a null builder


   T objCopy = obj.value();  // obj.value() returns a const reference to 
                             // the initialized value

It is important to follow this recipe carefully.  In particular,
the builder must be enclosed in a scope, as it's destructor
plays a crucial role in finalizing the initialization.


template<class T>
class Lazy {
public:
   Lazy();

   Lazy(const Lazy&);             // "deep" copies
   Lazy& operator=(const Lazy&);

   const T& value() const;

   ~Lazy();

   kill();  // destroy and reset

   class Builder {
      Builder(const Lazy&); 
     ~Builder()

      T& operator *  () const;
      T* operator -> () const;

      bool operator!() const; // test for null

   };
   


****************************************************************************/

#ifndef NTL_Lazy__H
#define NTL_Lazy__H


#include <NTL/tools.h>
#include <NTL/SmartPtr.h>
#include <NTL/thread.h>


NTL_OPEN_NNS



// FIXME: Right now, the builder mechanism is not exception safe.
// Although the destructor will release the mutex (if any), it
// mark the object as "constructed", even if it is not.
// If we want exception safety, we may have to modify the
// interface, so that we invoke builder.finalize() to 
// signal (to the destructor) that the object has been built.
// Care would have to be taken to ensure that we "roll back"
// to the right internal state in this case.
// NOTE: right now, NTL's tracevec builders would be the most
// prone to this problem, if we did introduce exceptions.


// NOTE: For more on double-checked locking, see
// http://preshing.com/20130930/double-checked-locking-is-fixed-in-cpp11/



template<class T>
class Lazy {
private:
   /* we make data members mutable so that Lazy members of
      other classes don't have to be. */

   mutable AtomicBool initialized; 
   mutable MutexProxy mtx;

   mutable UniquePtr<T> data;

public:
   Lazy() : initialized(false) { }

   void kill() 
   { 
      data.reset();
      initialized = false; 
   }

   Lazy& operator=(const Lazy& other) 
   {
      if (this == &other) return *this;

      kill();
      if (other.initialized) {
         data.make(*other.data);
         initialized = true;
      }
      return *this;
   }
   
   Lazy(const Lazy& other) : initialized(false)
   {
      *this = other;
   }

   const T& value() const { return *data; }

   class Builder {
   private:
      T *p;
      const Lazy& ref;
      GuardProxy guard;

      Builder(const Builder&); // disabled
      void operator=(const Builder&); // disabled



   public:
      Builder(const Lazy& _ref) : p(0), ref(_ref), guard(_ref.mtx)
      {
         // Double-checked locking
         if (ref.initialized || (guard.lock(), ref.initialized)) 
            return;

         ref.data.make();
         p = ref.data.get();
      }

      ~Builder() { if (p) ref.initialized = true; }

      T& operator *  () const { return *p; }
      T* operator -> () const { return p; }

      bool operator!() const { return !p; }
   };
};


NTL_CLOSE_NNS


#endif

