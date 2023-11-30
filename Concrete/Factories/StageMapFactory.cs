// using System;
// using System.Collections.Generic;
// using System.Linq;
// using static StageConstants;

// public class StageMapFactory
// {
//     private static readonly int[] _possibleGrandchildrenCount = [1,2];

//     public TreeNode<StageType> GenerateMapTemplate(int depth)
//     {
//         var start = new TreeNode<StageType>(StageType.CombatRegular);

//         start.AddChild(RandomStageType());
//         start.AddChild(RandomStageType());

//         var currLayer = new List<TreeNode<StageType>>() { start };

//         while (depth > 2)
//         {
//             var childrenCreated = 0;
//             foreach (var curr in currLayer)
//             {
//                 foreach (var child in curr.Children)
//                 {
//                     var grandchildrenCount = childrenCreated >= 2 ? 1 : _possibleGrandchildrenCount.PickRandomElement();
//                     for (int _ = 0; _ < grandchildrenCount; _++)
//                     {
//                         child.AddChildren(RandomStageType());
//                         childrenCreated++;
//                     }
//                 }
//             }

//             currLayer = currLayer.SelectMany(curr => curr.Children).ToList();
//             depth--;
//         }

//         currLayer = currLayer.SelectMany(curr => curr.Children).ToList();


        

//         // for (int i = 0; i < depth - 1; i++)
//         // {
//         //     var maxNodes = Math.Abs(depth - i) > i ? 2 : 3;
//         //     for (int j = 0; j < maxNodes; j++)
//         //     {
//         //         curr.AddChild(
//         //             Enum.GetValues<StageType>()
//         //                 .Where(t => t !=StageType.Boss)
//         //                 .PickRandomElement());
//         //         curr = curr.Children.Last();
//         //         if (curr == start)
//         //         {
                    
//         //         }
//         //     }
//         // }
//     }

//     private StageType RandomStageType() => 
//         Enum.GetValues<StageType>()
//             .Where(t => t !=StageType.Boss)
//             .PickRandomElement();
// }