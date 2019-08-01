## This is a quick refresher for python functions and syntax  ##

#print("Hello python!")

message = "This is a variable"
#print(message)

#print(message.title()) #This Is A Variable
#print(message.upper()) #THIS IS A VARIABLE
#print(message.lower()) #this is a variable

message2 = "This is a message"
#print(message + " " + message2)

language = "python    "
language.rstrip()
#print(language) #python
#strip() to remove whitespace from left and right side of the string


print(3**2) #9
print(2+3*4)
print((2+3)*4)
#python support order of expression

num = 42
print(message2 + " for agent " + str(num))

print(None == []) #False

# if num != 42:
#     raise AssertionError("num value not equal to 42")
# assert num > 45, "num less than 46"


##########  LISTS   #############################

items = ["tom","dick",'h',45,"harry"]
print(items[2])
items[2]="h_new"
items.append("newly added item")
print(items)
items.insert(2,"meddler")
del items[4]
print(items)
print (items.pop(2)) #pop 2nd element
items.remove("newly added item") # removing by value
print(items)


##### SORTING

print(sorted(items)) #without sorting the actual list
items.reverse() #simple reverse the list
print("reversed: " + str(items))
items.sort()
print(str(items) + " no of items: "+str(len(items)))



##### LOOPING
items.append(34)
items.append('d')
for ll in items:
    print(ll)


for val in range(94,100,2): #start inclusive, end exclusive, and jump value
    print(val)

digits = list(range(2,12))
print(digits)
print(sum(digits))
print(min(digits))
print(max(digits))


##### SLICING
sliceEnd = int(len(digits)/2)
print(digits[0:sliceEnd])       #digits[:sliceEnd]
print(digits[sliceEnd:])

#can not assign new list with = operator 
digits_copy = digits[:]  #copying list into new one...

#an immutable list is called a tuple
#tple = (100,40,60)
tple = tuple(range(45,52,2))
for tp in tple:
    print(tp)



########## Conditional Statement
tple = tuple(range(79,98,3))
if tple[1]==82 or tple[1]==83:
    print("tple[1] is 82 or 83 " + str(tple[1]))
else:
    print("tple[1] not 82 or 83")

if 95 in tple:
    print("tuple contains 95")
else:
    print("tuple doesn't contain 95")



########### Dictionary
dictionary = {"key1":"val1","key2":2,"key3":"val3"}
print(dictionary)
dictionary["key4"] = 54
del dictionary["key2"]

for key,val in dictionary.items():
    print("key: "+str(key)+"\tval: "+str(val))
#for key in dictionary.keys():
#for val in dictionary.values():

keys = list(dictionary.keys())
print(keys)





############ INPUTS

# name = input("username: ")
# print("Hi! "+name)


import random

def GetRandomInt():
    return random.randint(10,100)

print(GetRandomInt())

def UpdateList(items):
    for i in range(0,len(items)):
        items[i]+=1

def GetFormatedReport(name, age, percent):
    return "Hi, "+name+"!\nAge: "+str(age)+"\nYou have scored "+str(percent)+"%"

def SwapVal(x,y):
    return y,x
def SwapGlobalVal():
    global x,y
    x,y = y,x

itms = list(range(5,10,2))
UpdateList(itms)
print(itms)

print(GetFormatedReport("nfynt",24,95.8))
print(GetFormatedReport(percent=95.6,age=32,name="ransom"))
print(GetFormatedReport("nnfh",percent=34.7,age=55))

x,y = 5,10
#x,y = SwapVal(x,y)
SwapGlobalVal()
print("X "+str(x)+"\tY "+str(y))


#### Empty Tuple
cart = ("ford","merc","lexus")
*cars, = cart #starred assignment of tuple into list; same as cars = list(cart)
cars+= ['bmw']
print(cars)

