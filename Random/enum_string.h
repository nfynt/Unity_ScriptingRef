#define LIST_ENUM_MyEnum(m)  m(One,"fgdfOne"); m(Two,"Two");
#define LIST_ENUM_MyEnum2(m)  m(Dog,"Dog"); m(Cat,"Cat");

#define ENUM_CASE_STR(name, val) case name: return #val;

#define MAKE_TO_STRING_FUNC(enumType)                  \
    inline const char* to_string(enumType e) {         \
        switch (e) {                                   \
            LIST_ENUM_##enumType(ENUM_CASE_STR)     \
            default: return "Unknown " #enumType;      \
        }                                              \
    }
// clang-format on
enum MEnum
{
    One,
    Two
};

MAKE_TO_STRING_FUNC(MEnum);
