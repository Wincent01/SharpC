typedef struct Object
{
	char* (*ToString)(struct Object*me);

	signed int (*Equals)(struct Object* obj, struct Object*me);

	signed int (*GetHashCode)(struct Object*me);

	struct Object* (*GetType)(struct Object*me);

} Object;

typedef struct Main
{
	char* (*ToString)(struct Main*me);

	signed int (*Equals)(struct Object* obj, struct Main*me);

	signed int (*GetHashCode)(struct Main*me);

	signed int (*Add)(signed int asigned int b, struct Main*me);

	unsigned long long (*Add_)(unsigned long long aunsigned long long b, struct Main*me);

	struct Object* (*GetType)(struct Main*me);

} Main;

typedef struct Sign
{
	char* (*ToString)(struct Sign*me);

	signed int (*Equals)(struct Object* obj, struct Sign*me);

	signed int (*GetHashCode)(struct Sign*me);

	signed int (*ToUlong)(struct Sign*me);

	struct Object* (*GetType)(struct Sign*me);

	signed int Signature;
} Sign;

extern char* ObjectToString (struct Object*me);

extern signed int ObjectEquals (struct Object* obj, struct Object*me);

extern signed int ObjectEquals_ (struct Object* objA, struct Object* objB);

extern signed int ObjectReferenceEquals (struct Object* objA, struct Object* objB);

extern signed int ObjectGetHashCode (struct Object*me);

extern struct Object* ObjectGetType (struct Object*me);

extern signed int MainAdd (signed int a, signed int b, struct Main*me);

extern unsigned long long MainAdd_ (unsigned long long a, unsigned long long b, struct Main*me);

extern char* MainToString (struct Object*me);

extern signed int MainEquals (struct Object* obj, struct Object*me);

extern signed int MainGetHashCode (struct Object*me);

extern struct Object* MainGetType (struct Object*me);

extern signed int SignToUlong (struct Sign*me);

extern char* SignToString (struct Object*me);

extern signed int SignEquals (struct Object* obj, struct Object*me);

extern signed int SignGetHashCode (struct Object*me);

extern struct Object* SignGetType (struct Object*me);

struct Object* newObject ();

struct Main* newMain ();

struct Sign* newSign (signed int i);

char* ObjectToString (struct Object*me)
{
	return (char*) ( ((struct Object*) me)->GetType(me)->ToString(((struct Object*) me)->GetType(me)));
}

signed int ObjectEquals (struct Object* obj, struct Object*me)
{
	return (signed int) (RuntimeHelpersEquals_(obj, me));
}

signed int ObjectEquals_ (struct Object* objA, struct Object* objB)
{
	if ((unsigned int) (objB) != (unsigned int) (objA)) goto F36_1148;
	return (signed int) (1);
	if ((unsigned int) (objA) == 0) goto F312_1181;
	if ((unsigned int) (objB) == 1) goto F314_1195;
	return (signed int) (0);
	return (signed int) (((struct Object*) objA)->Equals(objB,objA));

	F36_1148:
	if ((unsigned int) (objA) == 0) goto F312_1181;
	if ((unsigned int) (objB) == 1) goto F314_1195;
	return (signed int) (0);
	return (signed int) (((struct Object*) objA)->Equals(objB,objA));

	F312_1181:
	return (signed int) (0);
	return (signed int) (((struct Object*) objA)->Equals(objB,objA));

	F314_1195:
	return (signed int) (((struct Object*) objA)->Equals(objB,objA));
}

signed int ObjectReferenceEquals (struct Object* objA, struct Object* objB)
{
	return (signed int) ((objB == objA));
}

signed int ObjectGetHashCode (struct Object*me)
{

	// Unknown function call -> [1] call -> System.Int32 System.Runtime.CompilerServices.RuntimeHelpers::GetHashCode(System.Object) | [1]
	return (signed int) (me);
}

struct Object* ObjectGetType (struct Object*me)
{
}

signed int MainAdd (signed int a, signed int b, struct Main*me)
{
	signed int var0 = 0;
	var0 = (signed int) ((a + b));
	goto F77_1153;
	return (signed int) (var0);

	F77_1153:
	return (signed int) (var0);
}

unsigned long long MainAdd_ (unsigned long long a, unsigned long long b, struct Main*me)
{
	unsigned long long var0 = 0;
	var0 = (unsigned long long) ((a + b));
	goto F87_1153;
	return (unsigned long long) (var0);

	F87_1153:
	return (unsigned long long) (var0);
}

char* MainToString (struct Object*me)
{
	return (char*) ( ((struct Object*) me)->GetType(me)->ToString(((struct Object*) me)->GetType(me)));
}

signed int MainEquals (struct Object* obj, struct Object*me)
{
	return (signed int) (RuntimeHelpersEquals_(obj, me));
}

signed int MainGetHashCode (struct Object*me)
{

	// Unknown function call -> [1] call -> System.Int32 System.Runtime.CompilerServices.RuntimeHelpers::GetHashCode(System.Object) | [1]
	return (signed int) (me);
}

struct Object* MainGetType (struct Object*me)
{
}

signed int SignToUlong (struct Sign*me)
{
	signed int var0 = 0;
	var0 = (signed int) (((struct Main*) newMain())->Add(8me->Signature,newMain()));
	goto F1321_1152;
	return (signed int) (var0);

	F1321_1152:
	return (signed int) (var0);
}

char* SignToString (struct Object*me)
{
	return (char*) ( ((struct Object*) me)->GetType(me)->ToString(((struct Object*) me)->GetType(me)));
}

signed int SignEquals (struct Object* obj, struct Object*me)
{
	return (signed int) (RuntimeHelpersEquals_(obj, me));
}

signed int SignGetHashCode (struct Object*me)
{

	// Unknown function call -> [1] call -> System.Int32 System.Runtime.CompilerServices.RuntimeHelpers::GetHashCode(System.Object) | [1]
	return (signed int) (me);
}

struct Object* SignGetType (struct Object*me)
{
}

struct Object* newObject ()
{
	Object* me = malloc(sizeof(Object));
	me->ToString = &ObjectToString;
	me->Equals = &ObjectEquals;
	me->GetHashCode = &ObjectGetHashCode;
	me->GetType = &ObjectGetType;

	return me;
}

struct Main* newMain ()
{
	Main* me = malloc(sizeof(Main));
	me->Add = &MainAdd;
	me->Add_ = &MainAdd_;
	me->ToString = &MainToString;
	me->Equals = &MainEquals;
	me->GetHashCode = &MainGetHashCode;
	me->GetType = &MainGetType;

/* Error call -> System.Void System.Object::.ctor() */

	return me;
}

struct Sign* newSign (signed int i)
{
	Sign* me = malloc(sizeof(Sign));
	me->ToUlong = &SignToUlong;
	me->ToString = &SignToString;
	me->Equals = &SignEquals;
	me->GetHashCode = &SignGetHashCode;
	me->GetType = &SignGetType;

/* Error call -> System.Void System.Object::.ctor() */
	me->Signature = i;

	return me;
}

