shader_type canvas_item;

uniform float percentage:hint_range(0.0,1,.01);
uniform float masked_alpha:hint_range(0,1,.01)=1;
uniform float unmasked_alpha:hint_range(0,1,.01);
uniform bool horizontal=false;
uniform bool invert=false;


void fragment() {
	vec4 main_tex=texture(TEXTURE,UV);
	COLOR.a=main_tex.a;
	
	float uvPOS=horizontal ? UV.y : UV.x;
	float step_value=unmasked_alpha+step(uvPOS,invert ? 1.0f - percentage : percentage);	
	step_value=invert ? 1.0f - step_value : step_value;

	COLOR.a=(main_tex.a)*step_value*masked_alpha;
}