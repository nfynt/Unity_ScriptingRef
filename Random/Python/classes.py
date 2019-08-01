import random
import math

class Math():
    def __init__(self, val1, val2):
        self.val1 = val1
        self.val2 = val2

    def GetVersion(self):
        return "v0.1"

    def GetNumSquared(self, num):
        return num ** 2

    def GetAuthorName(self):
        return "Nfynt"

    def GetNumCubed(self, num):
        return num ** 3

    def GetRandomNum(self):
        return random.randint(89,8989)

    ''' Get a raise to the power of b '''
    def GetPow(self, a, b):
        return a ** b

    def ShowVals(self):
        print(self.val1)    
        print(self.val2)


#### Simple inheritence
class Equations(Math):
    def __init__(self, val1, val2, val3):
        super().__init__(val1,val2)
        self.val3 = val3

    def GetSquaredRoot(self, num):
        return math.sqrt(num)