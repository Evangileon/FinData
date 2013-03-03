using UserFunc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;


namespace Luajit4CSharpTest
{
    
    
    /// <summary>
    ///这是 LuajitLuajitTest 的测试类，旨在
    ///包含所有 LuajitLuajitTest 单元测试
    ///</summary>
    [TestClass()]
    public class LuajitLuajitTest
    {
        //private IntPtr L = new IntPtr();
        private Luajit L = new Luajit();

        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //    //this
        //}
        
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        [TestInitialize()]
        public void MyTestInitialize()
        {
            this.lua = new Luajit();
        }
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


/// <summary>
///Luajit 构造函数 的测试
///</summary>
[TestMethod()]
public void LuajitConstructorLuajitTest()
{
    Luajit target = new Luajit();
    Assert.Inconclusive("TODO: 实现用来验证目标的代码");
}

/// <summary>
///luaJIT_setmode 的测试
///</summary>
[TestMethod()]
public void luaJIT_setmodeLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
int mode = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.luaJIT_setmode(L, idx, mode);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_call 的测试
///</summary>
[TestMethod()]
public void lua_callLuajitTest()
{
 // TODO: 初始化为适当的值
int nargs = 0; // TODO: 初始化为适当的值
int nresults = 0; // TODO: 初始化为适当的值
    Luajit.lua_call(L, nargs, nresults);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_checkstack 的测试
///</summary>
[TestMethod()]
public void lua_checkstackLuajitTest()
{
 // TODO: 初始化为适当的值
int sz = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_checkstack(L, sz);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_close 的测试
///</summary>
[TestMethod()]
public void lua_closeLuajitTest()
{
 // TODO: 初始化为适当的值
    Luajit.lua_close(L);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_concat 的测试
///</summary>
[TestMethod()]
public void lua_concatLuajitTest()
{
 // TODO: 初始化为适当的值
int n = 0; // TODO: 初始化为适当的值
    Luajit.lua_concat(L, n);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_createtable 的测试
///</summary>
[TestMethod()]
public void lua_createtableLuajitTest()
{
 // TODO: 初始化为适当的值
int narr = 0; // TODO: 初始化为适当的值
int nrec = 0; // TODO: 初始化为适当的值
    Luajit.lua_createtable(L, narr, nrec);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_equal 的测试
///</summary>
[TestMethod()]
public void lua_equalLuajitTest()
{
 // TODO: 初始化为适当的值
int idx1 = 0; // TODO: 初始化为适当的值
int idx2 = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_equal(L, idx1, idx2);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_error 的测试
///</summary>
[TestMethod()]
public void lua_errorLuajitTest()
{
 // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_error(L);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_gc 的测试
///</summary>
[TestMethod()]
public void lua_gcLuajitTest()
{
 // TODO: 初始化为适当的值
int what = 0; // TODO: 初始化为适当的值
int data = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_gc(L, what, data);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_getallocf 的测试
///</summary>
[TestMethod()]
public void lua_getallocfLuajitTest()
{
 // TODO: 初始化为适当的值
IntPtr ud = new IntPtr(); // TODO: 初始化为适当的值
Luajit.lua_Alloc expected = null; // TODO: 初始化为适当的值
    Luajit.lua_Alloc actual;
    actual = Luajit.lua_getallocf(L, ud);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_getfenv 的测试
///</summary>
[TestMethod()]
public void lua_getfenvLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
    Luajit.lua_getfenv(L, idx);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_getfield 的测试
///</summary>
[TestMethod()]
public void lua_getfieldLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
string k = string.Empty; // TODO: 初始化为适当的值
    Luajit.lua_getfield(L, idx, k);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_getmetatable 的测试
///</summary>
[TestMethod()]
public void lua_getmetatableLuajitTest()
{
 // TODO: 初始化为适当的值
int objindex = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_getmetatable(L, objindex);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_gettable 的测试
///</summary>
[TestMethod()]
public void lua_gettableLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
    Luajit.lua_gettable(L, idx);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_gettop 的测试
///</summary>
[TestMethod()]
public void lua_gettopLuajitTest()
{
 // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_gettop(L);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_insert 的测试
///</summary>
[TestMethod()]
public void lua_insertLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
    Luajit.lua_insert(L, idx);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_iscfunction 的测试
///</summary>
[TestMethod()]
public void lua_iscfunctionLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_iscfunction(L, idx);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_isnumber 的测试
///</summary>
[TestMethod()]
public void lua_isnumberLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_isnumber(L, idx);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_isstring 的测试
///</summary>
[TestMethod()]
public void lua_isstringLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_isstring(L, idx);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_isuserdata 的测试
///</summary>
[TestMethod()]
public void lua_isuserdataLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_isuserdata(L, idx);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_lessthan 的测试
///</summary>
[TestMethod()]
public void lua_lessthanLuajitTest()
{
 // TODO: 初始化为适当的值
int idx1 = 0; // TODO: 初始化为适当的值
int idx2 = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_lessthan(L, idx1, idx2);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_newthread 的测试
///</summary>
[TestMethod()]
public void lua_newthreadLuajitTest()
{
 // TODO: 初始化为适当的值
IntPtr expected = new IntPtr(); // TODO: 初始化为适当的值
    IntPtr actual;
    actual = Luajit.lua_newthread(L);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_newuserdata 的测试
///</summary>
[TestMethod()]
public void lua_newuserdataLuajitTest()
{
 // TODO: 初始化为适当的值
int sz = 0; // TODO: 初始化为适当的值
IntPtr expected = new IntPtr(); // TODO: 初始化为适当的值
    IntPtr actual;
    actual = Luajit.lua_newuserdata(L, sz);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_next 的测试
///</summary>
[TestMethod()]
public void lua_nextLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_next(L, idx);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_objlen 的测试
///</summary>
[TestMethod()]
public void lua_objlenLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_objlen(L, idx);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_pcall 的测试
///</summary>
[TestMethod()]
public void lua_pcallLuajitTest()
{
 // TODO: 初始化为适当的值
int nargs = 0; // TODO: 初始化为适当的值
int nresults = 0; // TODO: 初始化为适当的值
int errfunc = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_pcall(L, nargs, nresults, errfunc);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_pushboolean 的测试
///</summary>
[TestMethod()]
public void lua_pushbooleanLuajitTest()
{
 // TODO: 初始化为适当的值
int b = 0; // TODO: 初始化为适当的值
    Luajit.lua_pushboolean(L, b);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_pushfstring 的测试
///</summary>
[TestMethod()]
public void lua_pushfstringLuajitTest()
{
 // TODO: 初始化为适当的值
string fmt = string.Empty; // TODO: 初始化为适当的值
object[] argp = null; // TODO: 初始化为适当的值
string expected = string.Empty; // TODO: 初始化为适当的值
    string actual;
    actual = Luajit.lua_pushfstring(L, fmt, argp);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_pushinteger 的测试
///</summary>
[TestMethod()]
public void lua_pushintegerLuajitTest()
{
 // TODO: 初始化为适当的值
int n = 0; // TODO: 初始化为适当的值
    Luajit.lua_pushinteger(L, n);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_pushlightuserdata 的测试
///</summary>
[TestMethod()]
public void lua_pushlightuserdataLuajitTest()
{
 // TODO: 初始化为适当的值
IntPtr p = new IntPtr(); // TODO: 初始化为适当的值
    Luajit.lua_pushlightuserdata(L, p);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_pushlstring 的测试
///</summary>
[TestMethod()]
public void lua_pushlstringLuajitTest()
{
 // TODO: 初始化为适当的值
string s = string.Empty; // TODO: 初始化为适当的值
int l = 0; // TODO: 初始化为适当的值
    Luajit.lua_pushlstring(L, s, l);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_pushnil 的测试
///</summary>
[TestMethod()]
public void lua_pushnilLuajitTest()
{
 // TODO: 初始化为适当的值
    Luajit.lua_pushnil(L);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_pushnumber 的测试
///</summary>
[TestMethod()]
public void lua_pushnumberLuajitTest()
{
 // TODO: 初始化为适当的值
double n = 0F; // TODO: 初始化为适当的值
    Luajit.lua_pushnumber(L, n);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_pushstring 的测试
///</summary>
[TestMethod()]
public void lua_pushstringLuajitTest()
{
 // TODO: 初始化为适当的值
string s = string.Empty; // TODO: 初始化为适当的值
    Luajit.lua_pushstring(L, s);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_pushthread 的测试
///</summary>
[TestMethod()]
public void lua_pushthreadLuajitTest()
{
 // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_pushthread(L);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_pushvalue 的测试
///</summary>
[TestMethod()]
public void lua_pushvalueLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
    Luajit.lua_pushvalue(L, idx);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_pushvfstring 的测试
///</summary>
[TestMethod()]
public void lua_pushvfstringLuajitTest()
{
 // TODO: 初始化为适当的值
string fmt = string.Empty; // TODO: 初始化为适当的值
object[] argp = null; // TODO: 初始化为适当的值
string expected = string.Empty; // TODO: 初始化为适当的值
    string actual;
    actual = Luajit.lua_pushvfstring(L, fmt, argp);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_rawequal 的测试
///</summary>
[TestMethod()]
public void lua_rawequalLuajitTest()
{
 // TODO: 初始化为适当的值
int idx1 = 0; // TODO: 初始化为适当的值
int idx2 = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_rawequal(L, idx1, idx2);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_rawget 的测试
///</summary>
[TestMethod()]
public void lua_rawgetLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
    Luajit.lua_rawget(L, idx);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_rawgeti 的测试
///</summary>
[TestMethod()]
public void lua_rawgetiLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
int n = 0; // TODO: 初始化为适当的值
    Luajit.lua_rawgeti(L, idx, n);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_rawset 的测试
///</summary>
[TestMethod()]
public void lua_rawsetLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
    Luajit.lua_rawset(L, idx);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_rawseti 的测试
///</summary>
[TestMethod()]
public void lua_rawsetiLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
int n = 0; // TODO: 初始化为适当的值
    Luajit.lua_rawseti(L, idx, n);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_remove 的测试
///</summary>
[TestMethod()]
public void lua_removeLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
    Luajit.lua_remove(L, idx);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_replace 的测试
///</summary>
[TestMethod()]
public void lua_replaceLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
    Luajit.lua_replace(L, idx);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_resume 的测试
///</summary>
[TestMethod()]
public void lua_resumeLuajitTest()
{
 // TODO: 初始化为适当的值
int narg = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_resume(L, narg);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_setfenv 的测试
///</summary>
[TestMethod()]
public void lua_setfenvLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_setfenv(L, idx);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_setfield 的测试
///</summary>
[TestMethod()]
public void lua_setfieldLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
string k = string.Empty; // TODO: 初始化为适当的值
    Luajit.lua_setfield(L, idx, k);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_setmetatable 的测试
///</summary>
[TestMethod()]
public void lua_setmetatableLuajitTest()
{
 // TODO: 初始化为适当的值
int objindex = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_setmetatable(L, objindex);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_settable 的测试
///</summary>
[TestMethod()]
public void lua_settableLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
    Luajit.lua_settable(L, idx);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_settop 的测试
///</summary>
[TestMethod()]
public void lua_settopLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
    Luajit.lua_settop(L, idx);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_status 的测试
///</summary>
[TestMethod()]
public void lua_statusLuajitTest()
{
 // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_status(L);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_toboolean 的测试
///</summary>
[TestMethod()]
public void lua_tobooleanLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_toboolean(L, idx);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_tocfunction 的测试
///</summary>
[TestMethod()]
public void lua_tocfunctionLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
Luajit.lua_CFunction expected = null; // TODO: 初始化为适当的值
    Luajit.lua_CFunction actual;
    actual = Luajit.lua_tocfunction(L, idx);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_tointeger 的测试
///</summary>
[TestMethod()]
public void lua_tointegerLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_tointeger(L, idx);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_tolstring 的测试
///</summary>
[TestMethod()]
public void lua_tolstringLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
IntPtr len = new IntPtr(); // TODO: 初始化为适当的值
string expected = string.Empty; // TODO: 初始化为适当的值
    string actual;
    actual = Luajit.lua_tolstring(L, idx, len);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_tonumber 的测试
///</summary>
[TestMethod()]
public void lua_tonumberLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
double expected = 0F; // TODO: 初始化为适当的值
    double actual;
    actual = Luajit.lua_tonumber(L, idx);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_topointer 的测试
///</summary>
[TestMethod()]
public void lua_topointerLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
IntPtr expected = new IntPtr(); // TODO: 初始化为适当的值
    IntPtr actual;
    actual = Luajit.lua_topointer(L, idx);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_tothread 的测试
///</summary>
[TestMethod()]
public void lua_tothreadLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
IntPtr expected = new IntPtr(); // TODO: 初始化为适当的值
    IntPtr actual;
    actual = Luajit.lua_tothread(L, idx);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_touserdata 的测试
///</summary>
[TestMethod()]
public void lua_touserdataLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
IntPtr expected = new IntPtr(); // TODO: 初始化为适当的值
    IntPtr actual;
    actual = Luajit.lua_touserdata(L, idx);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_type 的测试
///</summary>
[TestMethod()]
public void lua_typeLuajitTest()
{
 // TODO: 初始化为适当的值
int idx = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_type(L, idx);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_typename 的测试
///</summary>
[TestMethod()]
public void lua_typenameLuajitTest()
{
 // TODO: 初始化为适当的值
int tp = 0; // TODO: 初始化为适当的值
string expected = string.Empty; // TODO: 初始化为适当的值
    string actual;
    actual = Luajit.lua_typename(L, tp);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///lua_xmove 的测试
///</summary>
[TestMethod()]
public void lua_xmoveLuajitTest()
{
IntPtr from = new IntPtr(); // TODO: 初始化为适当的值
IntPtr to = new IntPtr(); // TODO: 初始化为适当的值
int n = 0; // TODO: 初始化为适当的值
    Luajit.lua_xmove(from, to, n);
    Assert.Inconclusive("无法验证不返回值的方法。");
}

/// <summary>
///lua_yield 的测试
///</summary>
[TestMethod()]
public void lua_yieldLuajitTest()
{
 // TODO: 初始化为适当的值
int nresults = 0; // TODO: 初始化为适当的值
int expected = 0; // TODO: 初始化为适当的值
    int actual;
    actual = Luajit.lua_yield(L, nresults);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}

/// <summary>
///normal_lua_Alloc 的测试
///</summary>
[TestMethod()]
[DeploymentItem("UserFunc.dll")]
public void normal_lua_AllocLuajitTest()
{
Luajit_Accessor target = new Luajit_Accessor(); // TODO: 初始化为适当的值
IntPtr ud = new IntPtr(); // TODO: 初始化为适当的值
IntPtr ptr = new IntPtr(); // TODO: 初始化为适当的值
int osize = 0; // TODO: 初始化为适当的值
int nsize = 0; // TODO: 初始化为适当的值
IntPtr expected = new IntPtr(); // TODO: 初始化为适当的值
    IntPtr actual;
    actual = target.normal_lua_Alloc(ud, ptr, osize, nsize);
    Assert.AreEqual(expected, actual);
    Assert.Inconclusive("验证此测试方法的正确性。");
}
    }
}
