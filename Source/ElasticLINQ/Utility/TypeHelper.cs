﻿// Copyright (c) Tier 3 Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 

using System;
using System.Collections.Generic;
using System.Linq;

namespace ElasticLinq.Utility
{
    internal static class TypeHelper
    {
        public static Type GetSequenceElementType(Type sequenceType)
        {
            var elementType = FindIEnumerable(sequenceType);
            return elementType == null
                ? sequenceType
                : elementType.GetGenericArguments()[0];
        }

        public static Type FindIEnumerable(Type sequenceType)
        {
            while (true)
            {
                if (sequenceType == null || sequenceType == typeof(string))
                    return null;

                if (sequenceType.IsArray)
                    return typeof(IEnumerable<>).MakeGenericType(sequenceType.GetElementType());

                foreach (var argument in sequenceType.GetGenericArguments())
                {
                    var candidateIEnumerable = typeof(IEnumerable<>).MakeGenericType(argument);
                    if (candidateIEnumerable.IsAssignableFrom(sequenceType))
                        return candidateIEnumerable;
                }

                foreach (var candidateInterface in sequenceType.GetInterfaces().Select(FindIEnumerable))
                    if (candidateInterface != null)
                        return candidateInterface;

                if (sequenceType.BaseType == null || sequenceType.BaseType == typeof(object))
                    return null;

                sequenceType = sequenceType.BaseType;
            }
        }

        public static bool IsNullableType(Type type)
        {
            return type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}