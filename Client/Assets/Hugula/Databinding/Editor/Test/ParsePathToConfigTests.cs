using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Hugula.Databinding;
using Hugula.Databinding.Binder;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Hugula.Databinding;

namespace Tests
{
    public class ParsePathToConfigTests
    {
        [Test]
        public void SimplePropertyPath()
        {
            // 输入: "context.on_click"
            // 输出: partConfigs = [
            //   { path = "context", isIndexer=false, isExpSetter=false, isSelf=false },
            //   { path = "on_click", isIndexer=false, isExpSetter=false, isSelf=false }
            // ]
            var parser = new Binding();
            parser.ParsePathToConfig("context.on_click");

            var parts = InvokeParseTokensField(parser, "partConfigs");
            Assert.AreEqual(2, parts.Length);

            Assert.AreEqual("context", parts[0].path);
            Assert.IsFalse(parts[0].isIndexer);
            Assert.IsFalse(parts[0].isExpSetter);

            Assert.AreEqual("on_click", parts[1].path);
            Assert.IsFalse(parts[1].isIndexer);
            Assert.IsFalse(parts[1].isExpSetter);
        }

        [Test]
        public void WithIndexer()
        {
            // 输入: "goods.color[2]"
            // 输出:
            // partConfigs = [
            //   { path = "goods", isIndexer=false, isExpSetter=false, isSelf=false },
            //   { path = "color", isIndexer=false, isExpSetter=false, isSelf=false },
            //   { path = "2",     isIndexer=true,  isExpSetter=false, isSelf=false }
            // ]
            var parser = new Binding();
            parser.ParsePathToConfig("goods.color[2]");

            var parts = InvokeParseTokensField(parser, "partConfigs");

            Assert.AreEqual(3, parts.Length);

            Assert.AreEqual("goods", parts[0].path);
            Assert.IsFalse(parts[0].isIndexer);

            Assert.AreEqual("color", parts[1].path);
            Assert.IsFalse(parts[1].isIndexer);

            Assert.AreEqual("2", parts[2].path);
            Assert.IsTrue(parts[2].isIndexer);
        }

        [Test]
        public void WithMethodCall()
        {
            // 输入: "get_text()"
            // 期望输出:
            // partConfigs = [
            //   { path = "get_text", isIndexer=false, isMethod=true, isSelf=false }
            // ]
            var parser = new Binding();
            parser.ParsePathToConfig("get_text()");

            var parts = InvokeParseTokensField(parser, "partConfigs");
            Assert.AreEqual(1, parts.Length);

            Assert.AreEqual("get_text", parts[0].path);
            Assert.IsFalse(parts[0].isIndexer);
            Assert.IsTrue(parts[0].isMethod);
        }

        [Test]
        public void WithSpecialExpression()
        {
            // 输入: "VM.bag.$(item1)"
            // 期望解析:
            // tokens = ["VM", "bag", "$(item1)"]
            // partConfigs:
            //   { path = "VM",        isIndexer=false, isExpSetter=false, isSelf=false }
            //   { path = "bag",       isIndexer=false, isExpSetter=false, isSelf=false }
            //   { path = "$(item1)",  isIndexer=false, isExpSetter=true,  isSelf=false } // 因为带(), isExpSetter=true
            var parser = new Binding();
            parser.ParsePathToConfig("VM.bag.$(.item1)");

            var parts = InvokeParseTokensField(parser, "partConfigs");
            Assert.AreEqual(3, parts.Length);

            Assert.AreEqual("VM", parts[0].path);
            Assert.IsFalse(parts[0].isExpSetter);

            Assert.AreEqual("bag", parts[1].path);
            Assert.IsFalse(parts[1].isExpSetter);

            Assert.AreEqual("item1", parts[2].path);//
            Assert.IsTrue(parts[2].isExpSetter);
            Assert.IsFalse(parts[2].isGlobal);

        }

           [Test]
        public void WithSpecialSetExpression()
        {
            // 输入: "$(VM.bag.on_item_render)"
            // 期望解析:
            // tokens = ["$(VM.bag.on_item_render)"]
            // partConfigs:
            //   { path = "VM.bag.on_item_render",     isGlobal=ture,   isIndexer=false, isExpSetter=true, isSelf=false }
            var parser = new Binding();
            parser.ParsePathToConfig("$(VM.bag.on_item_render)");

            var parts = InvokeParseTokensField(parser, "partConfigs");
            Assert.AreEqual(1, parts.Length);

            Assert.AreEqual("VM.bag.on_item_render", parts[0].path);
            Assert.IsTrue(parts[0].isExpSetter);
            Assert.IsTrue(parts[0].isGlobal);

            // parser.ParsePathToConfig("on_item_render");
            // parts = InvokeParseTokensField(parser, "partConfigs");
            // Assert.AreEqual(1, parts.Length);
            // Assert.IsFalse(parts[0].isGlobal);

        }

          [Test]
        public void WithSpecialGetExpression()
        {
            // 输入: "&(VM.bag.on_item_render)"
            // 期望解析:
            // tokens = ["&(VM.bag.on_item_render)"]
            // partConfigs:
            //   { path = "VM.bag.on_item_render",     isGlobal=ture,   isIndexer=false, isExpSetter=true, isSelf=false }
            var parser = new Binding();
            parser.ParsePathToConfig("&(VM.bag.on_item_render)");

            var parts = InvokeParseTokensField(parser, "partConfigs");
            Assert.AreEqual(1, parts.Length);

            Assert.AreEqual("VM.bag.on_item_render", parts[0].path);
            Assert.IsTrue(parts[0].isExpGetter);
            Assert.IsTrue(parts[0].isGlobal);

            parser.ParsePathToConfig("&(.on_item_render)");
             parts = InvokeParseTokensField(parser, "partConfigs");
            Assert.AreEqual(1, parts.Length);

            Assert.AreEqual("on_item_render", parts[0].path);
            Assert.IsTrue(parts[0].isExpGetter);
            Assert.IsFalse(parts[0].isGlobal);

        }

        [Test]
        public void MixedSpecialExpressionAndIndexerAndMethod()
        {
            // 输入: "bag.$(show_item).&(.set_item).color[3].get_data()"
            // 期望：
            // tokens = ["bag", "$(show_item)", "&(.set_item)", "color[3]", "get_data()"]
            // parts:
            //   { "bag",            isIndexer=false, isExpSetter=false }
            //   { "$(show_item)",   isIndexer=false, isExpSetter=true  }
            //   { "&(.set_item)",   isIndexer=false, isExpSetter=true  }
            //   { "color",          isIndexer=false, isExpSetter=false }
            //   { "3",              isIndexer=true,  isExpSetter=false }
            //   { "get_data",       isIndexer=false, isExpSetter=true  }
            var parser = new Binding();
            parser.ParsePathToConfig("bag.$(show_item).&(.set_item).color[3].get_data()");

            var parts = InvokeParseTokensField(parser, "partConfigs");
            // parts应有6个:
            Assert.AreEqual(6, parts.Length);

            Assert.AreEqual("bag", parts[0].path);
            Assert.IsFalse(parts[0].isExpSetter);

            Assert.AreEqual("show_item", parts[1].path);
            Assert.IsTrue(parts[1].isExpSetter);
            Assert.IsTrue(parts[1].isGlobal);

            Assert.AreEqual("set_item", parts[2].path);
            Assert.IsTrue(parts[2].isExpGetter);
            Assert.IsFalse(parts[2].isGlobal);

            Assert.AreEqual("color", parts[3].path);
            Assert.IsFalse(parts[3].isExpSetter);

            Assert.AreEqual("3", parts[4].path);
            Assert.IsTrue(parts[4].isIndexer);

            Assert.AreEqual("get_data", parts[5].path);
            Assert.IsTrue(parts[5].isMethod);
        }

        private BindingPathPartConfig[] InvokeParseTokensField(Binding parser, string field)
        {
            // 假设ParseTokensWithSpecialExpressions为private，可使用反射或将其改为internal测试
            // 或者将其提取到公共辅助类中直接调用
            var field1 = typeof(Binding).GetField(field, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (BindingPathPartConfig[])field1.GetValue(parser); //Invoke(parser, new object[] { input });
        }

    }

    [TestFixture]
    public class ParseTokensWithSpecialExpressionsTests
    {
        [Test]
        public void NormalPath_NoSpecialExpression()
        {
            // 输入: "context.on_click"
            // 输出: ["context", "on_click"]
            var parser = new Binding(); // 替换为实际类名
            var tokens = InvokeParseTokensMethod(parser, "context.on_click");

            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual("context", tokens[0]);
            Assert.AreEqual("on_click", tokens[1]);
        }

        [Test]
        public void HasIndexer()
        {
            // 输入: "goods.color[2]"
            // 输出: ["goods", "color[2]"]
            var parser = new Binding();
            var tokens = InvokeParseTokensMethod(parser, "goods.color[2]");

            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual("goods", tokens[0]);
            Assert.AreEqual("color[2]", tokens[1]);
        }

        [Test]
        public void SpecialExpressionDollarBlock()
        {
            // 输入: "VM.bag.$(item1)"
            // 预期输出: ["VM", "bag", "$(item1)"]
            var parser = new Binding();
            var tokens = InvokeParseTokensMethod(parser, "VM.bag.$(item1)");

            Assert.AreEqual(3, tokens.Count);
            Assert.AreEqual("VM", tokens[0]);
            Assert.AreEqual("bag", tokens[1]);
            Assert.AreEqual("$(item1)", tokens[2]);
        }

        [Test]
        public void SpecialExpressionAmpBlock()
        {
            // 输入: "bag.&(events.click)"
            // 预期输出: ["bag", "&(events.click)"]
            var parser = new Binding();
            var tokens = InvokeParseTokensMethod(parser, "bag.&(events.click)");

            Assert.AreEqual(2, tokens.Count);
            Assert.AreEqual("bag", tokens[0]);
            Assert.AreEqual("&(events.click)", tokens[1]);
        }

        [Test]
        public void MixedMultipleSpecialBlocks()
        {
            // 输入: "bag.$(show_item).&(.set_item).color"
            // 预期输出: ["bag", "$(show_item)", "&(.set_item)", "color"]
            // 注意: $(...)中的内容是"show_item"
            //       &(...)中的内容是".set_item"
            var parser = new Binding();
            var tokens = InvokeParseTokensMethod(parser, "bag.$(show_item).&(.set_item).color");

            Assert.AreEqual(4, tokens.Count);
            Assert.AreEqual("bag", tokens[0]);
            Assert.AreEqual("$(show_item)", tokens[1]);
            Assert.AreEqual("&(.set_item)", tokens[2]);
            Assert.AreEqual("color", tokens[3]);
        }

        [Test]
        public void UnclosedSpecialBlock_ThrowException()
        {
            // 输入: "bag.$(not_closed"
            // 没有匹配的')'，应抛出异常
            var parser = new Binding();
            Assert.Throws<System.FormatException>(() => InvokeParseTokensMethod(parser, "bag.$(not_closed"));
        }

        // 辅助函数，用于调用私有的ParseTokensWithSpecialExpressions(如果是private可以通过反射或改为internal)
        private List<string> InvokeParseTokensMethod(Binding parser, string input)
        {
            // 假设ParseTokensWithSpecialExpressions为private，可使用反射或将其改为internal测试
            // 或者将其提取到公共辅助类中直接调用
            var method = typeof(Binding).GetMethod("ParseTokensWithSpecialExpressions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (List<string>)method.Invoke(parser, new object[] { input });
        }

    }

}