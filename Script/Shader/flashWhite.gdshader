shader_type canvas_item;

uniform float white = 0;

void fragment() {
	COLOR.rgb = texture(TEXTURE, UV).rgb * (1.0 - white) + vec3(1, 1, 1) * white;
	COLOR.a = texture(TEXTURE, UV).a;
}
