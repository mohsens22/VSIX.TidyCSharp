﻿using System;

namespace Geeks.GeeksProductivityTools.Menus.Cleanup.Renaming
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using System.Linq.Expressions;
    using System.Reflection;
    internal abstract class SyntaxWrapper<TNode>
    {
        public static SyntaxWrapper<TNode> Default { get; } = FindDefaultSyntaxWrapper();

        public abstract TNode Wrap(SyntaxNode node);

        public abstract SyntaxNode Unwrap(TNode node);

        private static SyntaxWrapper<TNode> FindDefaultSyntaxWrapper()
        {
            if (typeof(SyntaxNode).GetTypeInfo().IsAssignableFrom(typeof(TNode).GetTypeInfo()))
            {
                return new DirectCastSyntaxWrapper();
            }

            return new ConversionSyntaxWrapper();
        }

        private sealed class DirectCastSyntaxWrapper : SyntaxWrapper<TNode>
        {
            public override SyntaxNode Unwrap(TNode node)
            {
                return (SyntaxNode)(object)node;
            }

            public override TNode Wrap(SyntaxNode node)
            {
                return (TNode)(object)node;
            }
        }

        private sealed class ConversionSyntaxWrapper : SyntaxWrapper<TNode>
        {
            private readonly Func<TNode, SyntaxNode> unwrapAccessor;
            private readonly Func<SyntaxNode, TNode> wrapAccessor;

            public ConversionSyntaxWrapper()
            {
                this.unwrapAccessor = LightupHelpers.CreateSyntaxPropertyAccessor<TNode, SyntaxNode>(typeof(TNode), nameof(ISyntaxWrapper<SyntaxNode>.SyntaxNode));

                var explicitOperator = typeof(TNode).GetTypeInfo().GetDeclaredMethods("op_Explicit")
                    .Single(m => m.ReturnType == typeof(TNode) && m.GetParameters()[0].ParameterType == typeof(SyntaxNode));
                var syntaxParameter = Expression.Parameter(typeof(SyntaxNode), "syntax");
                Expression<Func<SyntaxNode, TNode>> wrapAccessorExpression =
                    Expression.Lambda<Func<SyntaxNode, TNode>>(
                        Expression.Call(explicitOperator, syntaxParameter),
                        syntaxParameter);

                this.wrapAccessor = wrapAccessorExpression.Compile();
            }

            public override SyntaxNode Unwrap(TNode node)
            {
                return this.unwrapAccessor(node);
            }

            public override TNode Wrap(SyntaxNode node)
            {
                return this.wrapAccessor(node);
            }
        }
    }
}
